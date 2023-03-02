namespace AdminAPI.Messaging
{
    public interface IMessageConsumer
    {
        void ConsumeMessage(string queueName);
    }
}
