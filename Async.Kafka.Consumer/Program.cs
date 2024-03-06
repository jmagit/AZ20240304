using Confluent.Kafka;
using System.Text.Json;
using System.Threading;

if(args.Length < 1) {
    Console.WriteLine("Usage: .. mode groupName");
    return;
}

string brokerList = "localhost:9092";
string topicName = "sensores";
string mode = args[0].ToLower();
string groupName = mode; // args[1];

Console.WriteLine($"Started consumer, Ctrl-C to stop consuming");

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

using(var consumer = new ConsumerBuilder<Ignore, string>(config)
    // Note: All handlers are called on the main .Consume thread.
    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
    //.SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
    //.SetPartitionsAssignedHandler((c, partitions) => {
    //    Console.WriteLine(
    //        "Partitions incrementally assigned: [" +
    //        string.Join(',', partitions.Select(p => p.Partition.Value)) +
    //        "], all: [" +
    //        string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Partition.Value)) +
    //        "]");
    //})
    //.SetPartitionsRevokedHandler((c, partitions) => {
    //    var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
    //    Console.WriteLine(
    //        "Partitions incrementally revoked: [" +
    //        string.Join(',', partitions.Select(p => p.Partition.Value)) +
    //        "], remaining: [" +
    //        string.Join(',', remaining.Select(p => p.Partition.Value)) +
    //        "]");
    //})
    //.SetPartitionsLostHandler((c, partitions) => {
    //    Console.WriteLine($"Partitions were lost: [{string.Join(", ", partitions)}]");
    //})
    .Build()) {
    consumer.Subscribe(topicName);

    Action<ConsumeResult<Ignore, string>> procesa = consumeResult => {
        var evento = JsonSerializer.Deserialize<Evento>(consumeResult.Message.Value);
        Console.WriteLine($"Received {consumeResult.Offset}: {evento.origen} - {evento.msg} [{evento.enviado}]");
    };
    if(mode.StartsWith("calc")) {
        var calc = new Dictionary<string, long>();
        procesa = consumeResult => {
            var evento = JsonSerializer.Deserialize<Evento>(consumeResult.Message.Value);
            var key = evento.origen;
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
