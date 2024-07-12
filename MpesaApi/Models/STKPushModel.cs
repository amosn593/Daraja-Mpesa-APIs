namespace MpesaApi.Models
{

    public class STKPushModel
    {
        public long? BusinessShortCode { get; set; } = null;
        public string? Password { get; set; }
        public string? Timestamp { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        public string? TransactionType { get; set; } = "CustomerPayBillOnline";
        public int? Amount { get; set; } = 1;
        public long? PartyA { get; set; } = 254702240787;
        public long? PartyB { get; set; } = null;
        public long? PhoneNumber { get; set; } = 254702240787;
        public string? CallBackURL { get; set; } = "https://test.com/path";
        public string? AccountReference { get; set; } = "C#API";
        public string? TransactionDesc { get; set; } = "C#API";
    }


    public class STKPushResponseModel
    {
        public string? MerchantRequestID { get; set; }
        public string? CheckoutRequestID { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseDescription { get; set; }
        public string? CustomerMessage { get; set; }
    }


}
