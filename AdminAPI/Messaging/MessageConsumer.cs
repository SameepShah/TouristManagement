using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AdminAPI.Messaging
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageConsumer(string hostName, string username, string password)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = username,
                Password = password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
         
        }

        public string ConsumeMessage(string queueName)
        {
            string messageResponse = string.Empty;
            _channel.QueueDeclare(queue: queueName,
                               exclusive: false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                messageResponse = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", messageResponse);
            };
            _channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
            return messageResponse;
        }

    }
}
