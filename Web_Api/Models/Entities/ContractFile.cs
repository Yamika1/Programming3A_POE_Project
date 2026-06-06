namespace Web_Api.Models.Entities
{
    public class ContractFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
        public int ContractId { get; set; }
    }
}