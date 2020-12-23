# Power Lines Messaging
NuGet package to abstract RabbitMQ messaging capability.

## PreRequisites

- .NET Core 3.1

## Usage
Create an instance of a `BrokerUrl` class

```
string host = "localhost";
int port = 5672;
string username = "username";
string password = "password";
BrokerUrl brokerUrl = new BrokerUrl(host, port, username, password)
```

Set the type of queue to connect to.

```
QueueType queueType = QueueType.Worker;
// or
QueueType queueType = QueueType.Exchange;
```

### Sending a message
Create an instance of a `Sender` class and connect to queue.

```
Sender sender = new Sender();
await sender.CreateConnectionToQueue(queueType, brokerUrl.ToString(), "queue");
```

Send a message passing the message body as an instance of an `object` and optionally the name of the sender.

```
object obj = new object();
string senderName = "my-service"
sender.SendMessage(obj, senderName)
```

### Receiving a message
Create an instance of a `Consumer` class and connect to queue.

```
Consumer consumer = new Consumer();
await consumer.CreateConnectionToQueue(queueType, brokerUrl.ToString(), "queue");
```

Start listening for new messages passing a method to run on receipt of a message. 

```
public void ReceiveMessage(string message)
{
    Console.WriteLine(message);
}

...
consumer.Listen(new Action<string>(ReceiveMessage));
```
