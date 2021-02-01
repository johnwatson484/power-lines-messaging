using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace PowerLinesMessaging
{
    public class Sender : Channel, ISender
    {
        public Sender(RabbitMQ.Client.IConnection connection, SenderOptions options) : base(connection, options)
        {
            CreateQueue();
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

        private void CreateQueue()
        {
            switch (queueType)
            {
                case QueueType.Worker:
                    CreateWorkerQueue(queueName);
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

        private string GetExchangeName()
        {
            return queueType == QueueType.Worker ? "" : queueName;
        }        
    }
}
