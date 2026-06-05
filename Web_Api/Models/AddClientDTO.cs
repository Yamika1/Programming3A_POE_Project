namespace Web_Api.Models
{
    public class AddClientDTO
    {
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }

        public string EmailAddress { get; set; }

        public string Region { get; set; }
        public int ContactNumber { get; set; }
    }
}
