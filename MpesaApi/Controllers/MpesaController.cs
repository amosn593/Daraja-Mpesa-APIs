using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MpesaApi.Data;
using MpesaApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MpesaApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MpesaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly MpesaSetting _options;
        private readonly AppDbContext _appDbContext;

        public MpesaController(IHttpClientFactory httpClientFactory, IOptions<MpesaSetting> options, AppDbContext appDbContext)
        {
            _httpClient = httpClientFactory.CreateClient("Mpesa");
            _options = options.Value;
            _appDbContext = appDbContext;
        }
        // GET: api/<MpesaController>
        [HttpGet("GetToken")]
        public async Task<AccessTokenResponse> GetToken()
        {
            try
            {
                
                //Add Basic Auth
                var authenticationString = $"{_options.ConsumerKey}:{_options.ConsumerSecret}";
                var tokenB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));

                //Using RestSharp
                var _client = new RestClient("https://sandbox.safaricom.co.ke");
               

                //Check In Db If there is access Token
                var AccessToken = await _appDbContext.AccessTokenResponses
                    .OrderByDescending(x => x.Id).FirstAsync();

                if (AccessToken == null)
                {
                    //Make call
                    //Using RestSharp
                    var _request = new RestRequest("/oauth/v1/generate?grant_type=client_credentials", Method.Get);
                    _request.AddHeader("Authorization", $"Basic {tokenB64}");

                    var response = await _client.ExecuteAsync(_request);

                    var responseBody = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content!);


                    responseBody!.CreatedDate = DateTime.Now;
                    responseBody.ExpireDate = DateTime.Now.AddSeconds((double)responseBody.expires_in!);

                    await _appDbContext.AccessTokenResponses.AddAsync(responseBody);

                    await _appDbContext.SaveChangesAsync();

                    //Return results
                    return responseBody;
                }
                else
                {
                    if(AccessToken.ExpireDate > DateTime.Now)
                    {
                        return AccessToken;
                    }
                    else
                    {
                        //Make call
                        //Using RestSharp
                        var _request = new RestRequest("/oauth/v1/generate?grant_type=client_credentials", Method.Get);
                        _request.AddHeader("Authorization", $"Basic {tokenB64}");

                        var response = await _client.ExecuteAsync(_request);

                        var responseBody = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content!);

                        AccessToken.access_token = responseBody!.access_token;
                        AccessToken.CreatedDate = DateTime.Now;
                        AccessToken.ExpireDate = DateTime.Now.AddSeconds((double)responseBody!.expires_in!);

                        await _appDbContext.SaveChangesAsync();

                        return AccessToken;
                    }
                }

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET api/<MpesaController>/5
        [HttpGet("StkPush")]
        public async Task<IActionResult> StkPush()
        {
            try
            {
                //Get Token
                var Token = await GetToken();

                //Create New Instance of STKPush model and get password
                var stkPush = new STKPushModel();

                stkPush.Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

               
                stkPush.PartyB = _options.BusinessShortCode;
                stkPush.BusinessShortCode = _options.BusinessShortCode;

                var plainTextBytes = Encoding.UTF8.GetBytes(stkPush.BusinessShortCode + $"{_options.PassKey}" + stkPush.Timestamp);

                stkPush.Password = Convert.ToBase64String(plainTextBytes);

                //Serialize data
                // Serialize the data to JSON
                var json = JsonConvert.SerializeObject(stkPush);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine(json);

                //Make Post Request
                var client = new RestClient("https://sandbox.safaricom.co.ke");
                var request = new RestRequest("mpesa/stkpush/v1/processrequest");
                request.AddHeader("Authorization", $"Bearer {Token.access_token}");
                request.AddHeader("Content-Type", "application/json");
                request.AddStringBody(json, ContentType.Json);

                var response2 = await client.ExecutePostAsync(request);

                var DeserializedResponse = JsonConvert.DeserializeObject<STKPushResponseModel>(response2.Content!);

                return Ok(DeserializedResponse);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<MpesaController>
        [HttpPost("StkPushCallBackUrl")]
        public async Task<IActionResult> StkPushCallBackUrl([FromBody] StkResultObject stkResultObject)
        {
            try
            {
                var json = JsonConvert.SerializeObject(stkResultObject);

                Console.WriteLine(json);

                return Ok(stkResultObject);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<MpesaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MpesaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
