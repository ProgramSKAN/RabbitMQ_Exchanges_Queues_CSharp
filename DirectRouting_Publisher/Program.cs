using Common;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace DirectRouting_Publisher
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";
        private const string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";   
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName,type: ExchangeType.Direct,durable: false);
                channel.QueueDeclare(queue: CardPaymentQueueName, durable: true, exclusive: false, autoDelete: false, arguments:null);
                channel.QueueDeclare(queue: PurchaseOrderQueueName, durable: true, exclusive: false, autoDelete: false, arguments:null);

                channel.QueueBind(queue: CardPaymentQueueName, exchange: ExchangeName, routingKey: "CardPayment");
                channel.QueueBind(queue: PurchaseOrderQueueName, exchange: ExchangeName, routingKey: "PurchaseOrder");


                foreach (var i in Enumerable.Range(1, 10))
                {
                    var msg = new Payment { AmountToPay=25m, CardNumber=$"1234-{i}",Name=$"Jack_{i}"};
                    channel.BasicPublish(exchange: ExchangeName,//any queue bound to this excahnge will receive message
                                     routingKey: "CardPayment",
                                     basicProperties: null,
                                     body: msg.Serialize());
                    Console.WriteLine("Message Sent : {0} : {1} :{2}", msg.AmountToPay, msg.CardNumber,msg.Name);
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    var msg = new PurchaseOrder { AmountToPay=55m,CompanyName=$"ABC Ltd - {i}",PaymentDayTerms=1,PurchaseOrderNumber= $"1234-{i}" };
                    channel.BasicPublish(exchange: ExchangeName,//any queue bound to this excahnge will receive message
                                     routingKey: "PurchaseOrder",
                                     basicProperties: null,
                                     body: msg.Serialize());
                    Console.WriteLine("Message Sent : {0} : {1} :{2}", msg.CompanyName, msg.PurchaseOrderNumber, msg.AmountToPay);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
