using Common;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace PublishSubscribe_Publisher
{
    class Program
    {
        private const string ExchangeName = "PublishSubscribe_Exchange";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName, 
                                        type: ExchangeType.Fanout, //any message passed to this exchange will be distributed to any consumer that are listening
                                        durable: false);//durable: true > persisted to the disk

                foreach (var i in Enumerable.Range(1, 10))
                {
                    var msg = new User { Id = i, Name = "Hello_" + i.ToString() };
                    channel.BasicPublish(exchange: ExchangeName,//any queue bound to this excahnge will receive message
                                         routingKey: "",
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
