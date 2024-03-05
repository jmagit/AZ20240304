

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
                UserName = "admin", Password = "curso", 
                ClientProvidedName = "Async.Amqp.Receptor"
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
                var canal = (model as EventingBasicConsumer).Model;
                var body = Encoding.UTF8.GetString(ev.Body.ToArray());
                var message = JsonSerializer.Deserialize<MessageDTO>(body /*, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }*/);
                if(message == null ) {
                    throw new FormatException("Invalid JSON");
                }
                Thread.Sleep(message.Msg.Length * 100);
                if(message.Msg.EndsWith(address)) {
                    canal.BasicNack(ev.DeliveryTag, false, true);
                    //canal.BasicReject(ev.DeliveryTag, false);
                } else {
                    Store.Add(message);
                    canal.BasicAck(ev.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: "demo.saludos",
                                 // autoAck: true,
                                 autoAck: false,
                                 consumer: consumer);
            var rpcConsumer = new EventingBasicConsumer(channel);
            rpcConsumer.Received += (model, ev) => {
                var canal = (model as EventingBasicConsumer).Model;
                var body = Encoding.UTF8.GetString(ev.Body.ToArray());
                var message = JsonSerializer.Deserialize<MessageDTO>(body /*, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }*/);
                var props = ev.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var response = new MessageDTO(
                    $"Cuerpo: {message.Msg.ToUpper()} Enviado: {message.Enviado} Recibido: {DateTime.Now}",
                    address
                    );
                Store.Add(response);
                Thread.Sleep(message.Msg.Length * 100);
                canal.BasicPublish(exchange: string.Empty,
                     routingKey: props.ReplyTo,
                     basicProperties: replyProps,
                     body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)));
                canal.BasicAck(deliveryTag: ev.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "demo.peticiones",
                                 // autoAck: true,
                                 autoAck: false,
                                 consumer: rpcConsumer);

        }
    }
}
