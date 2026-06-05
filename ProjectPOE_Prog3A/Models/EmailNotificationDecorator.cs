namespace ProjectPOE_Prog3A.Models
{
    public class EmailNotificationDecorator : IDecorator
    {
        private readonly IDecorator _notification;

        public EmailNotificationDecorator(IDecorator notification)
        {
            _notification = notification;
        }

        public void Send(string message)
        {
            _notification.Send(message);
            Console.WriteLine("Email sent: " + message);
        }

    }
}