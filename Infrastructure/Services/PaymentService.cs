using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public abstract class PaymentService
    {
        protected readonly IConfiguration _configuration;
        protected readonly string BaseUrl;

        protected PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            BaseUrl = _configuration["Url:BaseUrl"];
        }

        protected string ComputeHmacSha256(string data, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        protected async Task<T> SendRequestAsync<T>(string url, object data)
        {
            var client = new RestClient(url);
            var request = new RestRequest { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddJsonBody(data);
            var response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
