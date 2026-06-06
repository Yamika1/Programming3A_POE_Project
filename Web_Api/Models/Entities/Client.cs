using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Api.Models.Entities
{
    [Table("Client")]
    public class Client
    {
        public int ClientId { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }

        public string Region { get; set; }

        public string ContactNumber { get; set; }

        public string EmailAddress { get; set; }
    }
}

