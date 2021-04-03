using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StandardQueue
{
    class Program
    {
        //private static ConnectionFactory _factory;
        //private static IConnection _connection;
        //private static IModel _channel;

        private const string QueueName = "StandardQueue_Example";

        public static void Main(string[] args)
        {
            //_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            var factory = new ConnectionFactory
            {
                Uri=new Uri("amqp://guest:guest@localhost:5672")
            };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                foreach (var i in Enumerable.Range(1,10))
                {
                    var msg = new User { Id = i, Name = "Hello_"+i.ToString() };
                    //SEND
                    //exchange is "", because we not defining custom exchnage. ""> to use default exchange.
                    //basicproperties is used to pass basic parameters into queue like corelationIds, replyToAddresses,...
                    channel.BasicPublish("", QueueName, null, msg.Serialize());//exchange,routingkey,basicproperties,body
                    Console.WriteLine("Message Sent : {0} : {1}", msg.Id, msg.Name);
                }

                //new QueueBasicConsumer() deprecated
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body.ToArray().DeSerialize(typeof(User));
                    Console.WriteLine("message received by consumer : " + body.ToString());
                    channel.BasicAck(e.DeliveryTag, false);
                };
                channel.BasicConsume(QueueName, true, consumer);

                var messageCount = Convert.ToInt32(channel.MessageCount(QueueName));
                //Thread.Sleep(1000 * messageCount);

                Console.WriteLine(" Connection closed, no more messages.");
                Console.ReadLine();
            }
            
        }

 


        private static uint GetMessageCount(IModel channel,string queueName)
        {
            //Declaring a Queue in RabbitMQ is Idempotent operation.ie.,it only be created it isn't already exists.
            //so, if QueueDeclare called multiple times, there will be no effect from second
            var results = channel.QueueDeclare(queueName, true, false, false, null);
            return results.MessageCount;
        }

    }
}
