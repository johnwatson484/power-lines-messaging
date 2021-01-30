using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace PowerLinesMessaging
{
    public class Sender : ISender
    {
        protected ConnectionFactory connectionFactory;
        protected RabbitMQ.Client.IConnection connection;
        protected IModel channel;
        protected QueueType queueType;
        protected string queue;


        public void CreateConnectionToQueue(SenderOptions options)
        {
            this.queueType = options.QueueType;
            this.queue = options.QueueName;
            CreateConnectionFactory(options.BrokerUrl);
            CreateConnection();
            CreateChannel();
            CreateQueue();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void SendMessage(object obj, string routingKey = "")
        {
            var message = JsonConvert.SerializeObject(obj);            
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: GetExchangeName(),
                                 routingKey: !string.IsNullOrEmpty(routingKey) ? routingKey : GetQueueName(),
                                 basicProperties: null,
                                 body: body);
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
                    break;
                case QueueType.ExchangeDirect:
                    CreateExchange(false);
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

        private string GetExchangeName()
        {
            return queueType == QueueType.Worker ? "" : queue;
        }

        private string GetQueueName()
        {
            return queueType == QueueType.Worker ? queue : "";
        }
    }
}
