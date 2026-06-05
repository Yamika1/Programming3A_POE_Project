namespace Web_Api.Models
{
    public class UpdateServiceRequestDto
    {
        public string? ServiceStatus { get; set; }
        public double ContractCost { get; set; }

        public string? RequestTypes { get; set; }

        public DateOnly RequestDate { get; set; }

        public string? RequestDescription { get; set; }

    }
}