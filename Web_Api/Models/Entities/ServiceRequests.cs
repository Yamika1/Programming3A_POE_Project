using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Api.Models.Entities
{
    [Table("ServiceRequests")]
    public class ServiceRequests
    {
        public int Id { get; set; }
        public string? ServiceStatus { get; set; }
        public double ContractCost { get; set; }

        public string? RequestTypes { get; set; }

        public DateOnly RequestDate { get; set; }

        public string? RequestDescription { get; set; }

    }
}