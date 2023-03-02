namespace BranchAPI.Messaging
{
    public interface IMessageProducer
    {
        public void SendMessage<T>(string queueName, T message);
    }
}
