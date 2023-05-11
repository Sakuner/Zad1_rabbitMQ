using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Zad1_MailObject;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri(uriString: "amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Consumer app";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);
//1 msg at the time 
channel.BasicQos(prefetchCount: 1, prefetchSize: 0, global: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(value: 2)).Wait();
    var body = args.Body.ToArray();

    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine("Sending message...");
    emailSender(message);

    channel.BasicAck(args.DeliveryTag, multiple: false);
};

string consumerTag = channel.BasicConsume(queueName, autoAck: false, consumer);

//readline to wait for msgs until enter key is pressed
Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
cnn.Close();

static void emailSender(string message)
{
    var msgObj = JsonSerializer.Deserialize<MailObject>(message);
    if(msgObj.Type == "SmtpEmail")
    {
        var mailMessage = msgObj.ToMailMessage();
        mailMessage.From = new MailAddress("kuba99wojciechowski@gmail.com");
        var client = new SmtpClient("smtp.gmail.com", 587);
        client.EnableSsl = true;
        //I deleted my credentials from here, but it works 
        client.Credentials = new NetworkCredential("", "");
        client.UseDefaultCredentials = false;
        client.Send(mailMessage);
    }
    //else for basic console type, for more types I would use switch case in this scenario
    else 
    {
        Console.WriteLine(msgObj.Content);
    }
}