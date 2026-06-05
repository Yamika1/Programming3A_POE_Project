namespace ProjectPOE_Prog3A.Models
{
    public class Manager
    {
        public int Id { get; set; }
        public string ManagerFirstName { get; set; }

        public string ManagerLastName { get; set; }

        public string EmailAddress { get; set; }

        public virtual List<ServiceRequests> ServiceRequests { get; set; }
        public virtual List<Client> clients { get; set; }

    }
}