using System;

namespace PowerLinesMessaging
{
    public interface IConsumer
    {
        void Listen(Action<string> messageAction);
    }
}
