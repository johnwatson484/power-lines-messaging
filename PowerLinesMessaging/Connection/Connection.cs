using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;

namespace PowerLinesMessaging
{
    public class Connection : IConnection
    {
        protected ConnectionFactory connectionFactory;
        protected RabbitMQ.Client.IConnection connection;
        protected List<Channel> channels;

        public void CreateConnection(ConnectionOptions options)
        {
            CreateConnectionFactory(options.BrokerUrl);
            CreateConnection();
            channels = new List<Channel>();
        }

        public Consumer CreateConsumerChannel(ConsumerOptions options)
        {
            ValidateChannelName(options.Name);
            var consumer = new Consumer(connection, options);
            channels.Add(consumer);
            return consumer;
        }

        public Sender CreateSenderChannel(SenderOptions options)
        {
            ValidateChannelName(options.Name);
            var sender = new Sender(connection, options);
            channels.Add(sender);
            return sender;
        }

        public void CloseChannel(string name)
        {
            channels.Where(x => x.Name == name).FirstOrDefault()?.CloseChannel();
        }

        public void CloseConnection()
        {
            channels.ForEach((channel) => { channel.CloseChannel(); });
            connection.Close();
        }

        protected void ValidateChannelName(string name)
        {
            if(string.IsNullOrEmpty(name) || channels.Where(x=>x.Name == name).Count() > 0)
            {
                throw new ArgumentException("Channel Name must be present and unique");
            }
        }

        private void CreateConnectionFactory(string brokerUrl)
        {
            connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(brokerUrl)
            };
        }
        private void CreateConnection()
        {
            connection = connectionFactory.CreateConnection();
        }
    }
}
