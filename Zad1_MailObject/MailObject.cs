using System.Net.Mail;
using System.Text.Json;

namespace Zad1_MailObject
{
    public class MailObject
    {
        public string Email { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static MailObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<MailObject>(json);
        }

        public MailMessage ToMailMessage()
        {
            var message = new MailMessage();
            message.To.Add(Email);
            message.Subject = $"{Title}";
            message.Body = $"{Content}";
            return message;
        }
    }
}