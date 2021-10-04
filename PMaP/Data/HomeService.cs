using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMaP.Models;
using PMaP.Models.Authenticate;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IHomeService
    {
        Task<HomeModel> PortfolioComposition();
    }

    public class HomeService : IHomeService
    {
        private IConfiguration _configuration;
        private ILocalStorageService _localStorageService;

        public HomeService(IConfiguration configuration, ILocalStorageService localStorageService)
        {
            _configuration = configuration;
            _localStorageService = localStorageService;
        }

        public async Task<HomeModel> PortfolioComposition()
        {
            HomeModel model = new HomeModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.GetConnectionString("pmapApiUrl"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                AuthenticateResponse authenticateResponse = new AuthenticateResponse();
                try
                {
                    authenticateResponse = await _localStorageService.GetItem<AuthenticateResponse>("user");
                }
                catch { }
                if (authenticateResponse != null && !string.IsNullOrEmpty(authenticateResponse.Token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticateResponse.Token);

                //HTTP GET
                var responseTask = await client.GetAsync("api/home");

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<HomeModel>(readTask);
                }
                model.ResponseCode = (int)result.StatusCode;
            }

            return model;
        }
    }
}
