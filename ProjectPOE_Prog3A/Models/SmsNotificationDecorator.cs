namespace ProjectPOE_Prog3A.Models
{
    public class SmsNotificationDecorator : IDecorator
    {
        private readonly IDecorator _notification;

        public SmsNotificationDecorator(IDecorator notification)
        {
            _notification = notification;
        }

        public void Send(string message)
        {
            _notification.Send(message);
            Console.WriteLine("SMS sent: " + message);
        }
    }
}