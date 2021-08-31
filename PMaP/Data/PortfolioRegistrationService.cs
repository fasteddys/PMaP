using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMaP.Models;
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
    public class PortfolioRegistrationService
    {
        private IConfiguration Configuration { get; }

        public PortfolioRegistrationService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<PortfolioModel> Index(Models.ViewModels.PortfolioValuation.ViewModel portfolioValuationViewModel, Summary summary)
        {
            if (portfolioValuationViewModel.PortfolioValuationAdd != null)
            {
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 = "";
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio = "";
            }

            PortfolioValuationModel portfolioValuationModel = new PortfolioValuationModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent content = new StringContent(JsonConvert.SerializeObject(portfolioValuationViewModel), Encoding.UTF8, "application/json");
                //HTTP POST
                var responseTask = await client.PostAsync("portfolioValuation/details/contracts", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuationModel = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            PortfolioModel portfolioModel = new PortfolioModel
            {
                ActiveTab = "summary",
                Contracts = portfolioValuationModel.Contracts,
                Participants = portfolioValuationModel.Participants,
                Investors = portfolioValuationModel.Investors,
                Summary = summary,
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
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                //HTTP POST
                var responseTask = await client.PostAsync("portfolioValuation/portfolio", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            return portfolioValuation;
        }

        public async Task<PortfolioValuationModel> UpdatePortfolio(PortfolioValuationModel model)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                //HTTP PUT
                var responseTask = await client.PutAsync("portfolioValuation/portfolio", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            return portfolioValuation;
        }

        public async Task<PortfolioValuationModel> DiscardPortfolio(Portfolio portfolio)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP DELETE
                var responseTask = await client.DeleteAsync("portfolioValuation/portfolio/" + portfolio.Id);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            return portfolioValuation;
        }

        public async Task<PortfolioValuationModel> DeletePortfolioContracts(int portfolioId, List<Contract> contracts)
        {
            PortfolioValuationModel portfolioValuation = new PortfolioValuationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent content = new StringContent(JsonConvert.SerializeObject(contracts), Encoding.UTF8, "application/json");
                //HTTP PUT
                var responseTask = await client.PutAsync("portfolioValuation/portfolio/" + portfolioId + "/contracts", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioValuation = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            return portfolioValuation;
        }
    }
}
