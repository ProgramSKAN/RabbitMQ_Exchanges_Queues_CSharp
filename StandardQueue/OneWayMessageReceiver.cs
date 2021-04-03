using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StandardQueue
{
    public class OneWayMessageReceiver:DefaultBasicConsumer
    {
        private readonly IModel _channel;
        public OneWayMessageReceiver(IModel channel)
        {
            _channel = channel;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var msg=body.ToArray().DeSerialize(typeof(User));
            Console.WriteLine("Message received by the consumer: "+ JsonConvert.SerializeObject(msg));
            Debug.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Debug.WriteLine(string.Concat("Content type: ", properties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            _channel.BasicAck(deliveryTag, false);
        }
    }
}
