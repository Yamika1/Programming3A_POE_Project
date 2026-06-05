namespace ProjectPOE_Prog3A.Models
{
    public class ConcreteObserver
    {
        public class Email : IObserver
        {

            public void Notify(string message)
            {
                Console.WriteLine($"Email : {message}");
            }
        }
        public class SMS : IObserver
        {

            public void Notify(string message)
            {
                Console.WriteLine($"SMS : {message}");
            }
        }
    }
}
