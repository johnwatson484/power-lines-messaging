using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PowerLinesMessaging
{
    public class Consumer : IConsumer
    {
        protected ConnectionFactory connectionFactory;
        protected RabbitMQ.Client.IConnection connection;
        protected IModel channel;
        protected QueueType queueType;
        protected string queue;
        protected string tempQueue;
        protected string serviceName;

        public void CreateConnectionToQueue(QueueType queueType, string brokerUrl, string queue, string serviceName = "")
        {
            this.queueType = queueType;
            this.queue = queue;
            this.serviceName = serviceName;
            CreateConnectionFactory(brokerUrl);
            CreateConnection();
            CreateChannel();
            CreateQueue();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void Listen(Action<string> messageAction)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                messageAction(message);
            };
            channel.BasicConsume(queue: GetQueueName(),
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void CreateConnectionFactory(string brokerUrl)
        {
            connectionFactory = new ConnectionFactory() {
                Uri = new Uri(brokerUrl)
            };
        }

        private void CreateConnection()
        {
            connection = connectionFactory.CreateConnection();
        }

        private void CreateChannel()
        {
            channel = connection.CreateModel();
        }

        private void CreateQueue()
        {
            switch (queueType)
            {
                case QueueType.Worker:
                    CreateWorkerQueue();
                    break;
                case QueueType.ExchangeFanout:
                    CreateExchange(true);
                    BindQueue();
                    break;
                case QueueType.ExchangeDirect:
                    CreateExchange(false);
                    BindQueue();
                    break;
                default:
                    break;
            }
        }

        private void CreateWorkerQueue()
        {
            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        private void CreateExchange(bool fanout = true)
        {
            channel.ExchangeDeclare(queue, fanout ? ExchangeType.Fanout : ExchangeType.Direct, true, false);
        }

        

        private void BindQueue()
        {
            tempQueue = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: tempQueue,
                              exchange: queue,
                              routingKey: GetExchangeRoutingKey());
        }

        private string GetQueueName()
        {
            return queueType == QueueType.Worker ? queue : tempQueue;
        }

        private string GetExchangeRoutingKey()	
        {	
            return queueType == QueueType.ExchangeDirect ? serviceName : "";	
        }
    }
}
