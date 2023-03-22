using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AdminAPI.Messaging
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDistributedCache _cache;
        public MessageConsumer(string hostName, string username, string password, IDistributedCache cache)
        {
            _cache = cache;
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = username,
                Password = password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            //ConsumeMessage("branch");
        }

        public void ConsumeMessage(string queueName)
        {
            _channel.QueueDeclare(queue: queueName,
                               exclusive: false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                //Consumer to Receive Data from Branch API
                var body = ea.Body.ToArray();
                var messageResponse = Encoding.UTF8.GetString(body);
                string? serializedData = null;
                byte[]? dataAsByteArray = null;
                if (messageResponse != null)
                {
                    var branches = JsonSerializer.Deserialize<List<Branch>>(messageResponse);
                    if (branches != null && branches.Count > 0)
                    {
                        //Set into RedisCache (Uncomment Once Azure Connection is Up)
                        serializedData = JsonSerializer.Serialize(branches);
                        dataAsByteArray = Encoding.UTF8.GetBytes(serializedData);
                        _cache.Set("branches", dataAsByteArray);
                    }
                }
            };
            _channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
            
        }

    }
}
