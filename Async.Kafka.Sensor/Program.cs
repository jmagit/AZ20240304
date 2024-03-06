using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;

if(args.Length != 1) {
    Console.WriteLine("Usage: .. sensorName");
    return;
}

string brokerList = "localhost:9092";
string topicName = "sensores";
string sensorName = args[0].ToUpper();

using(var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = brokerList }).Build()) {
    try {
        await adminClient.CreateTopicsAsync([
            new TopicSpecification { Name = topicName, ReplicationFactor = 1, NumPartitions = 2 }
        ]);
    } catch(CreateTopicsException e) {
        if(!e.Results[0].Error.Reason.Contains("already exists"))
            Console.WriteLine($"An error occurred creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
    }
}

var config = new ProducerConfig { BootstrapServers = brokerList };

using(var producer = new ProducerBuilder<string, string>(config).Build()) {
    Console.WriteLine("\n-----------------------------------------------------------------------");
    Console.WriteLine($"Producer {producer.Name} producing on topic {topicName} with key {sensorName}.");
    Console.WriteLine("-----------------------------------------------------------------------");

    for(int i = 1; i <= 100; i++) {
        try {
            var delay = Random.Shared.Next(500, 1500);
            Thread.Sleep(delay);
            var message =new Message<string, string> { 
                Key = sensorName,
                Value = $"{{\"msg\": \"Evento {i}\",\"origen\": \"{sensorName}\",\"enviado\": \"{DateTime.Now.ToString("s")}\"}}" 
                //Value = JsonSerializer.Serialize(new Evento($"Evento {i}", sensorName, DateTime.Now))
            };
            var deliveryReport = await producer.ProduceAsync(topicName, message);
            Console.WriteLine($"entrega ({delay / 1000.0}s): {deliveryReport.Offset} {message.Value}");
        } catch(ProduceException<string, string> e) {
            Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
        }
    }
}

record Evento(string msg, string origen, DateTime enviado);
