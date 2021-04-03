using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace DirectRouting_Subscriber1
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";
        private const string RoutingKey = "CardPayment";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //DeclareAndBindQueueToExchange
                    channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType.Direct, durable: false);//idempotent
                    channel.QueueDeclare(queue: CardPaymentQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: CardPaymentQueueName, exchange: ExchangeName, routingKey: RoutingKey);//this queue is bound to this exchange
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);//not to give >1 msgs at a time.or dont dispatch a new message until it is acked

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = (Payment)e.Body.ToArray().DeSerialize(typeof(Payment));
                        channel.BasicAck(e.DeliveryTag, false);
                        Console.WriteLine("message received by consumer : " + "routing-key" + e.RoutingKey + "message:" + body.ToString());
                    };
                    channel.BasicConsume(queue: CardPaymentQueueName,//subscribing this queue
                                         autoAck: false,
                                         consumer: consumer);
                    Console.WriteLine("CardPaymentDirectRouting_Queue Consumer Listening ....");

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
