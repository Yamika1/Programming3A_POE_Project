namespace Web_Api.Models
{
    public class AddContractDTO
    {
        public string ContractName { get; set; }

        public string ContractType { get; set; }
        public string ContractDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}