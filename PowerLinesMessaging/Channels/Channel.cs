using RabbitMQ.Client;

namespace PowerLinesMessaging;

public class Channel
{
    public string Name { get; private set; }
    protected IModel channel;
    protected RabbitMQ.Client.IConnection connection;
    protected QueueType queueType;
    protected string queueName;

    public Channel(RabbitMQ.Client.IConnection connection, Options options)
    {
        Name = options.Name;
        queueType = options.QueueType;
        queueName = options.QueueName;
        this.connection = connection;
        CreateChannel();
    }

    public void CloseChannel()
    {
        channel.Close();
    }

    private void CreateChannel()
    {
        channel = connection.CreateModel();
    }

    protected void CreateWorkerQueue(string queueName)
    {
        channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    protected void CreateExchange(bool fanout = true)
    {
        channel.ExchangeDeclare(queueName, fanout ? ExchangeType.Fanout : ExchangeType.Direct, true, false);
    }

    protected virtual string GetQueueName()
    {
        return queueType == QueueType.Worker ? queueName : "";
    }
}
