namespace ProjectPOE_Prog3A.Models
{
    public class Contracts
    {
        public int Id { get; set; }
        public string ContractName { get; set; }

        public string ContractDescription { get; set; }

        public string ContractType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ContractStatus ContractStatus { get; set; }

        public virtual List<Client> clients { get; set; }
        public List<ContractFile> Files { get; set; } = new List<ContractFile>();
    }
}
