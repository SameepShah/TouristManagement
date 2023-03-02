namespace AdminAPI.Messaging
{
    public interface IMessageConsumer
    {
        string ConsumeMessage(string queueName);
    }
}
