using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace PublishSubscribe_Subscriber
{
    class Program
    {
        private const string ExchangeName = "PublishSubscribe_Exchange";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //DeclareAndBindQueueToExchange
                    channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType.Fanout,durable:false);//idempotent
                    var queueName = channel.QueueDeclare().QueueName;//declaring system generated queuename.starts like "amq.gen.TsdoX9qziswm9QCbdkp9Zw"
                    channel.QueueBind(queue: queueName, exchange: ExchangeName,routingKey: "");//this queue is bound to this exchange

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = (User)e.Body.ToArray().DeSerialize(typeof(User));
                        Console.WriteLine("message received by consumer : " + body.ToString());
                        //channel.BasicAck(e.DeliveryTag, false);// here all messages are sent to every consumer (FANOUT), so dont need to acknowledge
                    };
                    channel.BasicConsume(queue: queueName,//subscribing this queue
                                         autoAck: true,//we are not waiting for message ack before receving next message.we don't need to in this case as this subscriber application is reading from its own queue, so it can take messages right away it can deal with.
                                         consumer: consumer);
                    Console.WriteLine("WorkerQueue Consumer Listening ....");

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
