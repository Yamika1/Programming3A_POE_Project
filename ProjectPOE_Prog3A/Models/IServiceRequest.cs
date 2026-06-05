namespace ProjectPOE_Prog3A.Models
{
    public interface IServiceRequest
    {
        void EnableNotifications(IObserver observer);
        void NotifyObservers(string message);
    }

}
