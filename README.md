[![Build Status](https://dev.azure.com/johnwatson484/John%20D%20Watson/_apis/build/status/Power%20Lines%20Messaging?branchName=main)](https://dev.azure.com/johnwatson484/John%20D%20Watson/_build/latest?definitionId=47&branchName=main)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=johnwatson484_power-lines-messaging&metric=alert_status)](https://sonarcloud.io/dashboard?id=johnwatson484_power-lines-messaging)

# Power Lines Messaging
NuGet package to abstract RabbitMQ messaging capability.

## PreRequisites
- .NET Core 8.0

## Usage
Create a new instance of a `Connection` with `ConnectionOptions`.

```csharp
var options = new ConnectionOptions
{
    Host = "localhost",
    Port = 5672
    Username = "username",
    Password = "password"
};
var connection = new Connection(options);
```

### Sending a message
Create a new Sender channel on a connection.

```csharp
var options = new SenderOptions
{
    Name = "My sending channel",
    QueueName = "my-queue",
    QueueType = QueueType.ExchangeFanout
};

var sender = connection.CreateSenderChannel(options);

object obj = new object();
sender.sendMessage(obj);

// if QueueType is `ExchangeDirect` then a routing key can be supplied
sender.sendMessage(obj, "routing-key");
```

The `QueueType` represents the target for publishing of which there are three options:
- `QueueType.Worker` - Worker queue
- `QueueType.ExchangeDirect` - Exchange of type direct where routing key can be supplied for message filtering
- `QueueType.ExchangeFanout` - Exchange of type fanout

### Receiving a message
Create a new Receiver channel on a connection.

```csharp
var options = new ConsumerOptions
{
    Name = "My receiving channel",
    QueueName = "my-queue",
    SubscriptionQueueName = "my-queue-subscription", // optional, if not supplied a temporary auto delete subscription queue is added.
    QueueType = QueueType.ExchangeFanout,
    RoutingKey = "routing-key" // optional, if QueueType is ExchangeDirect and a routing key is needed for message filtering.
};

var consumer = connection.CreateConsumerChannel(options);

consumer.Listen(new Action<string>(ReceiveMessage));

...
public void ReceiveMessage(string message)
{
    Console.WriteLine(message);
}
```

### Closing a connection
As RabbitMQ uses TCP connections must be terminated by the client application when they are no longer needed or when the application is terminated.

```csharp
// close a single channel
connection.CloseChannel("My channel name");

// close all channels and the connection
connection.CloseConnection();
```

### Best practice
- Creating a new connection is expensive, use at most two connections, one for sending and one for receiving
- Keep connections open rather than frequently opening and closing them
- Ensure connections are safely closed in line with the application lifecycle
