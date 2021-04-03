using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace DirectRouting_Subscriber2
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";
        private const string RoutingKey = "PurchaseOrder";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //DeclareAndBindQueueToExchange
                    channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType.Direct, durable: false);//idempotent
                    channel.QueueDeclare(queue: PurchaseOrderQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: PurchaseOrderQueueName, exchange: ExchangeName, routingKey: RoutingKey);//this queue is bound to this exchange
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = (PurchaseOrder)e.Body.ToArray().DeSerialize(typeof(PurchaseOrder));
                        channel.BasicAck(e.DeliveryTag, false);
                        Console.WriteLine("message received by consumer : " + "routing-key" + e.RoutingKey + "message:" + body.ToString());
                    };
                    channel.BasicConsume(queue: PurchaseOrderQueueName,//subscribing this queue
                                         autoAck: false,
                                         consumer: consumer);
                    Console.WriteLine("PurchaseOrderDirectRouting_Queue Consumer Listening ....");

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
