using System;

namespace PowerLinesMessaging
{
    public interface IConsumer
    {
        void CreateConnectionToQueue(ConsumerOptions options);

        void CloseConnection();

        void Listen(Action<string> messageAction);
    }
}
