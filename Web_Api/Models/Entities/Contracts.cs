using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Api.Models.Entities
{
    [Table("Contracts")]
    public class Contracts
    {

        public int Id { get; set; }
        public string ContractName { get; set; }

        public string ContractDescription { get; set; }

        public string ContractType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public int ContractStatus { get; set; } = 0;
    }
}