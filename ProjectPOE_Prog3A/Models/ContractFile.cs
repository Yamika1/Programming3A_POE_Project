using System.Diagnostics.Contracts;

namespace ProjectPOE_Prog3A.Models
{
    public class ContractFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }

        public int ContractId { get; set; }
        public Contracts Contracts { get; set; }

        public bool IsValid(string fileName, long fileSize)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return false;

            if (fileSize >= 5_000_000)
                return false;

            return true;
        }
    }
}