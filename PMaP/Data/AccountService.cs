using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMaP.Models.Authenticate;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest request);
    }

    public class AccountService : IAccountService
    {
        private IConfiguration _configuration { get; }
        private ILocalStorageService _localStorageService;

        public AccountService(IConfiguration configuration, ILocalStorageService localStorageService)
        {
            _configuration = configuration;
            _localStorageService = localStorageService;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            AuthenticateResponse authenticateResponse = new AuthenticateResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.GetConnectionString("pmapApiUrl"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                //HTTP GET
                var responseTask = await client.PostAsync("api/users/authenticate", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(readTask);
                    await _localStorageService.SetItem("user", authenticateResponse);
                }
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(readTask);
                }
                authenticateResponse.RespCode = (int)result.StatusCode;
            }

            return authenticateResponse;
        }
    }
}
