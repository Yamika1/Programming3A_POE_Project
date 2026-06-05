namespace Web_Api.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double Rate { get; set; }
    }
}