﻿using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text;
using System.Text.Json;
using System.Threading;

if(args.Length < 1) {
    Console.WriteLine("Usage: .. mode groupName");
    return;
}

string brokerList = "localhost:9092";
string topicName = "sensores";
string mode = args[0].ToLower();
string groupName = $"consumidor-{(args.Length > 1 ? args[1] : mode)}";

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


Console.WriteLine($"Started consumer {mode.ToUpper()}, Ctrl-C to stop consuming");

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};
var cancellationToken = cts.Token;

var config = new ConsumerConfig {
    BootstrapServers = brokerList,
    GroupId = groupName,
    EnableAutoOffsetStore = false,
    EnableAutoCommit = true,
    StatisticsIntervalMs = 5000,
    SessionTimeoutMs = 6000,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnablePartitionEof = true,
    PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
};

using(var consumer = new ConsumerBuilder<string, Evento>(config)
    .SetValueDeserializer(new EventoDeserializer())
    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
    //.SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json.Replace(", \"", ",\n\t\"")}"))
    .SetPartitionsAssignedHandler((c, partitions) => {
        Console.WriteLine(
            $"Partitions incrementally assigned: [{string.Join(", ", partitions.Select(p => p.Partition.Value))}], " +
            $" all: [{string.Join(", ", c.Assignment.Concat(partitions).Select(p => p.Partition.Value))}]");
    })
    .SetPartitionsRevokedHandler((c, partitions) => {
        var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
        Console.WriteLine(
            $"Partitions incrementally revoked: [{string.Join(", ", partitions.Select(p => p.Partition.Value))}], " +
            $" remaining: [{string.Join(", ", remaining.Select(p => p.Partition.Value))}]");
    })
    .SetPartitionsLostHandler((c, partitions) => {
        Console.WriteLine($"Partitions were lost: [{string.Join(", ", partitions)}]");
    })
    .Build()) {
    consumer.Subscribe(topicName);

    Action<ConsumeResult<string, Evento>> procesa = consumeResult => {
        //var evento = JsonSerializer.Deserialize<Evento>(consumeResult.Message.Value);
        var evento = consumeResult.Message.Value;
        Console.WriteLine($"Received {consumeResult.Offset}: {evento.origen} - {evento.msg} [{evento.enviado}]");
    };
    if(mode.StartsWith("calc")) {
        var calc = new Dictionary<string, long>();
        procesa = consumeResult => {
            //var evento = JsonSerializer.Deserialize<Evento>(consumeResult.Message.Value);
            var evento = consumeResult.Message.Value;
            var key = consumeResult.Message.Key; // evento.origen;
            if(calc.ContainsKey(key))
                calc[key]++;
            else
                calc[key] = 1;
            Console.WriteLine("\nResumen\n=====================");
            foreach(var item in calc) {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        };
    }
    try {
        while(true) {
            try {
                var consumeResult = consumer.Consume(cancellationToken);

                if(consumeResult.IsPartitionEOF) {
                    Console.WriteLine(
                        $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                    continue;
                }
                procesa(consumeResult);
                try {
                    consumer.StoreOffset(consumeResult);
                } catch(KafkaException e) {
                    Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                }
            } catch(ConsumeException e) {
                Console.WriteLine($"Consume error: {e.Error.Reason}");
            }
        }
    } catch(OperationCanceledException) {
        Console.WriteLine("Closing consumer.");
        consumer.Close();
    }
}

record Evento(string msg, string origen, DateTime enviado);

#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
class EventoDeserializer : IDeserializer<Evento> {
    public Evento Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) {
        if(isNull) return null;
        return JsonSerializer.Deserialize<Evento>(Encoding.UTF8.GetString(data));
    }
}
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
