using RabbitMQ.Client;
using System.Text;
using Zad1_MailObject;

//in asp.net app I would try to fit connection settings in appsettings.json
ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri(uriString:"amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Producer app";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);


//test message - in real system it would have to get its content from different source
var msgTest = new MailObject
{
    Type = "SmtpEmail",
    Email = "sakuner@wp.pl",
    Title = "Title",
    Content = "test msg 123"
};

var jsonMsg = msgTest.ToJson();

byte[] messageBodyBytes = Encoding.UTF8.GetBytes(jsonMsg);
channel.BasicPublish(exchangeName, routingKey, basicProperties: null, messageBodyBytes);


channel.Close();
cnn.Close();