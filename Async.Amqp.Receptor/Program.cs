

using Async.Amqp.Emisor.Models;
using Async.Amqp.Receptor.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Async.Amqp.Receptor {
    // dotnet watch run --urls=http://localhost:8055/
    // dotnet watch run --urls=http://localhost:8056/

    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if(app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            // app.Run();
            app.Start();
            ConsumerConfig(app.Urls.First().ToString().Split(':').Last());
            app.WaitForShutdown();
        }

        private static void ConsumerConfig(string address) {
            var factory = new ConnectionFactory {
                HostName = "localhost", Port = 5672,
                UserName = "admin", Password = "curso", ClientProvidedName = "app:audit component:event-consumer"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "demo.saludos",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ev) => {
                var body = Encoding.UTF8.GetString(ev.Body.ToArray());
                var message = JsonSerializer.Deserialize<MessageDTO>(body /*, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }*/);
                if(message.Msg.EndsWith(address)) {
                    //channel.BasicNack(ev.DeliveryTag, false, true);
                    channel.BasicReject(ev.DeliveryTag, false);
                } else {
                    Store.Add(message);
                    channel.BasicAck(ev.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: "demo.saludos",
                                 // autoAck: true,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}
