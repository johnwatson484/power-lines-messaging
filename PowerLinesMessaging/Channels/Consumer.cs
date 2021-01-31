using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PowerLinesMessaging
{
    public class Consumer : Channel, IConsumer
    {        
        protected string subscriptionQueue;
        protected string routingKey;

        public Consumer(RabbitMQ.Client.IConnection connection, ConsumerOptions options) : base(connection, options)
        {
            this.routingKey = options.RoutingKey;
            this.subscriptionQueue = options.SubscriptionQueueName;
            CreateQueueBinding();
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

        private void CreateQueueBinding()
        {
            switch (queueType)
            {
                case QueueType.Worker:
                    CreateWorkerQueue(queueName);
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

        private void BindQueue()
        {
            if (subscriptionQueue != null)
            {
                CreateWorkerQueue(subscriptionQueue);
            }
            else
            {
                subscriptionQueue = channel.QueueDeclare().QueueName;
            }

            channel.QueueBind(queue: subscriptionQueue,
                              exchange: queueName,
                              routingKey: GetExchangeRoutingKey());
        }

        protected override string GetQueueName()
        {
            return queueType == QueueType.Worker ? queueName : subscriptionQueue;
        }

        private string GetExchangeRoutingKey()
        {
            return queueType == QueueType.ExchangeDirect ? routingKey : "";
        }
    }
}
