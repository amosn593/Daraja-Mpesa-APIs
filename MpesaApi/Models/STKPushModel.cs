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
        public string? CallBackURL { get; set; } = "https://n1qtx2lp-7299.euw.devtunnels.ms/api/Mpesa/StkPushCallBackUrl";
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


    public class StkResultObject
    {
        public Body Body { get; set; }
    }

    public class Body
    {
        public stkCallback stkCallback { get; set; }
    }

    public class stkCallback
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public Callbackmetadata CallbackMetadata { get; set; }
    }

    public class Callbackmetadata
    {
        public Item[] Item { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }



}
