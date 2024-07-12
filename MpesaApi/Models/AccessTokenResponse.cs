namespace MpesaApi.Models;

public class AccessTokenResponse
{
    public int Id { get; set; }
    public string? access_token { get; set; }
    public int? expires_in { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }
}


