using Common;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace WorkerQueueProducer
{
    public class Program
    {
        private const string QueueName = "WorkerQueue_Queue";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost",UserName="guest",Password="guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                foreach (var i in Enumerable.Range(1, 100))
                {
                    var msg = new User { Id = i, Name = "Hello_" + i.ToString() };
                    channel.BasicPublish(exchange: "",
                                     routingKey: QueueName,
                                     basicProperties: null,
                                     body: msg.Serialize());
                    Console.WriteLine("Message Sent : {0} : {1}", msg.Id, msg.Name);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
