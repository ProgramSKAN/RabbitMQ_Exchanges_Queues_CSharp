using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace WorkerQueueConsumer
{
    class Program
    {
        private const string QueueName = "WorkerQueue_Queue";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: QueueName,//QueueDeclare is idempotent
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    channel.BasicQos(prefetchSize:0, prefetchCount:1, global:false);//Qos:QUality of service
                    //prefetchCount:1 > RabbitMQ won't dispatch a new message to consumer until that consumer finished processing an acknowledged message.RabbitMQ will instead dispatch a message to the next worker that is not busy.
                    //prefetchCount:1 > more equal and fair dispatch of messages between workers.

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = e.Body.ToArray().DeSerialize(typeof(User));
                        Console.WriteLine("message received by consumer : " + body.ToString());
                        channel.BasicAck(e.DeliveryTag, false);//it means worker finished processing message and ready to process next.
                    };
                    channel.BasicConsume(queue: QueueName,
                                         autoAck: false,
                                         consumer: consumer);
                    Console.WriteLine("WorkerQueue Consumer Listening ....");



                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
