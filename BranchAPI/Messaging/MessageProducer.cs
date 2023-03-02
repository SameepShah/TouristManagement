using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace BranchAPI.Messaging
{
    public class MessageProducer : IMessageProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageProducer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void SendMessage<T>(string queueName, T message)
        {
            _channel.QueueDeclare(queueName, exclusive: false);

            //Serialize the message
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            //put the data on to the queue
            _channel.BasicPublish(exchange: "", routingKey: queueName, body: body);
        }
    }
}
