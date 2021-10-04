using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMaP.Models;
using PMaP.Models.Authenticate;
using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IPortfolioRegistrationService
    {
        Task<PortfolioModel> Index(Models.ViewModels.PortfolioValuation.ViewModel portfolioValuationViewModel);
        Task<PortfolioValuationModel> AddPortfolio(PortfolioModel model);
        Task<PortfolioValuationModel> UpdatePortfolio(PortfolioValuationModel model);
        Task<PortfolioValuationModel> DiscardPortfolio(Portfolio portfolio);
        Task<PortfolioValuationModel> DeletePortfolioContracts(int portfolioId, List<Contract> contracts);
    }

    public class PortfolioRegistrationService : IPortfolioRegistrationService
    {
        private IConfiguration Configuration { get; }
        private ILocalStorageService _localStorageService;

        public PortfolioRegistrationService(IConfiguration configuration, ILocalStorageService localStorageService)
        {
            Configuration = configuration;
            _localStorageService = localStorageService;
        }

        public async Task<PortfolioModel> Index(Models.ViewModels.PortfolioValuation.ViewModel portfolioValuationViewModel)
        {
            if (portfolioValuationViewModel.PortfolioValuationAdd != null)
            {
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 = "";
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio = "";
            }

            PortfolioValuationModel portfolioValuationModel = new PortfolioValuationModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("pmapApiUrl"));
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

                HttpContent content = new StringContent(JsonConvert.SerializeObject(portfolioValuationViewModel), Encoding.UTF8, "application/json");
                //HTTP POST
                var responseTask = await client.PostAsync("api/portfolioEvaluation/details/contracts", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuationModel = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
                portfolioValuationModel.ResponseCode = (int)result.StatusCode;
            }

            PortfolioModel portfolioModel = new PortfolioModel
            {
                ResponseCode = portfolioValuationModel.ResponseCode,
                ActiveTab = "summary",
                Contracts = portfolioValuationModel.Contracts,
                Participants = portfolioValuationModel.Participants,
                Investors = portfolioValuationModel.Investors,
                Procedures = portfolioValuationModel.Procedures,
                Summary = portfolioValuationModel.Summary,
                ViewModel = new ViewModel
                {
                    DateAdded = DateTime.Now,
                    PortfolioId = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Id : 0,
                    Portfolio = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 : "",
                    Subportfolio = portfolioValuationViewModel.PortfolioValuationAdd != null? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio : ""
                }
            };

            return portfolioModel;
        }

        public async Task<PortfolioValuationModel> AddPortfolio(PortfolioModel model)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("pmapApiUrl"));
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

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                //HTTP POST
                var responseTask = await client.PostAsync("api/portfolioEvaluation/portfolio", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
                portfolioValuation.ResponseCode = (int)result.StatusCode;
            }

            return portfolioValuation;
        }

        public async Task<PortfolioValuationModel> UpdatePortfolio(PortfolioValuationModel model)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("pmapApiUrl"));
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

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                //HTTP PUT
                var responseTask = await client.PutAsync("api/portfolioEvaluation/portfolio", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
                portfolioValuation.ResponseCode = (int)result.StatusCode;
            }

            return portfolioValuation;
        }

        public async Task<PortfolioValuationModel> DiscardPortfolio(Portfolio portfolio)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("pmapApiUrl"));
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

                //HTTP DELETE
                var responseTask = await client.DeleteAsync("api/portfolioEvaluation/portfolio/" + portfolio.Id);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
                portfolioValuation.ResponseCode = (int)result.StatusCode;
            }

            return portfolioValuation;
        }

        public async Task<PortfolioValuationModel> DeletePortfolioContracts(int portfolioId, List<Contract> contracts)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("pmapApiUrl"));
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

                HttpContent content = new StringContent(JsonConvert.SerializeObject(contracts), Encoding.UTF8, "application/json");
                //HTTP PUT
                var responseTask = await client.PutAsync("api/portfolioEvaluation/portfolio/" + portfolioId + "/contracts", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
                portfolioValuation.ResponseCode = (int)result.StatusCode;
            }

            return portfolioValuation;
        }
    }
}
