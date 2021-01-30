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
        protected string subscriptionQueue;
        protected string routingKey;

        public void CreateConnectionToQueue(ConsumerOptions options)
        {
            this.queueType = options.QueueType;
            this.queue = options.QueueName;
            this.routingKey = options.RoutingKey;
            this.subscriptionQueue = options.SubscriptionQueueName;
            CreateConnectionFactory(options.BrokerUrl);
            CreateConnection();
            CreateChannel();
            CreateQueueBinding();
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

        private void CreateQueueBinding()
        {
            switch (queueType)
            {
                case QueueType.Worker:
                    CreateQueue(queue);
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

        private void CreateQueue(string queueName)
        {
            channel.QueueDeclare(queue: queueName,
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
            if(subscriptionQueue != null)
            {
                CreateQueue(subscriptionQueue);
            }
            else
            {
                subscriptionQueue = channel.QueueDeclare().QueueName;
            }

            channel.QueueBind(queue: subscriptionQueue,
                              exchange: queue,
                              routingKey: GetExchangeRoutingKey());
        }

        private string GetQueueName()
        {
            return queueType == QueueType.Worker ? queue : subscriptionQueue;
        }

        private string GetExchangeRoutingKey()	
        {	
            return queueType == QueueType.ExchangeDirect ? routingKey : "";	
        }
    }
}
