using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PMaP.Models;
using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IPortfolioMarketService
    {
        Task<PortfolioMarketModel> Portfolios(string queryStrings);
    }

    public class PortfolioMarketService : IPortfolioMarketService
    {
        private IConfiguration Configuration { get; }
        private IHttpService _httpService;
        //public static int _portfolioId;

        public PortfolioMarketService(IConfiguration configuration, IHttpService httpService)
        {
            Configuration = configuration;
            _httpService = httpService;
        }

        public async Task<PortfolioMarketModel> Portfolios(string queryStrings)
        {
            try
            {
                var model = await _httpService.Get<IEnumerable<PortfolioMarket>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/PortfolioMarket/GetAllQuery" + queryStrings) ?? new List<PortfolioMarket>();

                List<SelectListItem> portfolioList = new List<SelectListItem>();
                portfolioList.Add(new SelectListItem() { Text = "Select", Value = "" });
                List<SelectListItem> subportfolioList = new List<SelectListItem>();
                subportfolioList.Add(new SelectListItem() { Text = "Select", Value = "" });
                if (model != null && model.Count() > 0)
                {
                    var portfolios = model.GroupBy(x => x.Project).Select(x => x.First()).OrderBy(x => x.Project).ToList();
                    foreach (var item in portfolios)
                    {
                        portfolioList.Add(new SelectListItem() { Text = item.Project, Value = item.Project });
                    }
                }

                return new PortfolioMarketModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    ViewModel = new ViewModel
                    {
                        PortfolioList = portfolioList,
                        SubportfolioList = subportfolioList
                    },
                    PortfolioMarkets = model.ToList()
                };
            }
            catch (Exception ex)
            {
                return new PortfolioMarketModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        //public async Task<PortfolioModel> Characteristics(PortfolioModel model)
        //{
        //    _portfolioId = 0;
        //    ViewModel viewModel = model.ViewModel;
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioUri"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //HTTP GET
        //        string portfolio = model.ViewModel.PortfolioList.Find(x => x.Text == model.ViewModel.Portfolio)?.Value;
        //        string subportfolio = model.ViewModel.SubportfolioList.Find(x => x.Text == model.ViewModel.Subportfolio)?.Value;
        //        var responseTask = await client.GetAsync("portfolios?portfolio=" + portfolio + "&subportfolio=" + subportfolio);

        //        var result = responseTask;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = await result.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<PortfolioModel>(readTask);
        //        }
        //    }
        //    model.ViewModel = viewModel;

        //    model.ViewModel.OperationType = model.ViewModel.OBCutOff = model.ViewModel.OBSingning = model.ViewModel.OBClosing = "";
        //    model.ViewModel.DateAdded = model.ViewModel.DateCutOff = model.ViewModel.DateSigning = model.ViewModel.DateClosing = null;
        //    if (model.ResponseCode == 200 && model.Portfolios != null && model.Portfolios.Count() > 0)
        //    {
        //        var portfolio = model.Portfolios.First();
        //        model.ViewModel.OperationType = portfolio.OperationType;
        //        model.ViewModel.OBCutOff = portfolio.CutOffOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
        //        model.ViewModel.OBSingning = portfolio.SigningOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
        //        model.ViewModel.OBClosing = portfolio.ClosingOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
        //        model.ViewModel.DateAdded = portfolio.CreationDate;
        //        model.ViewModel.DateCutOff = portfolio.CutOffDate;
        //        model.ViewModel.DateSigning = portfolio.SigningDate;
        //        model.ViewModel.DateClosing = portfolio.ClosingDate;
        //        _portfolioId = portfolio.Id;
        //    }

        //    return model;
        //}

        //public async Task<ContractsModel> Assessment()
        //{
        //    ContractsModel model = new ContractsModel { Summary = new Summary() };

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Configuration.GetConnectionString("contractUri"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //HTTP GET
        //        var responseTask = await client.GetAsync("contracts/portfolio/" + _portfolioId + "/assessment");

        //        var result = responseTask;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = await result.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<ContractsModel>(readTask);
        //        }
        //    }

        //    return model;
        //}

        //public async Task<ContractsModel> Contracts()
        //{
        //    ContractsModel model = new ContractsModel();

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Configuration.GetConnectionString("contractUri"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //HTTP GET
        //        var responseTask = await client.GetAsync("contracts/portfolio/" + _portfolioId);

        //        var result = responseTask;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = await result.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<ContractsModel>(readTask);
        //        }
        //    }

        //    return model;
        //}

        //public async Task<ParticipantsModel> Participants()
        //{
        //    ParticipantsModel model = new ParticipantsModel();

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Configuration.GetConnectionString("participantUri"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //HTTP GET
        //        var responseTask = await client.GetAsync("participants/portfolio/" + _portfolioId);

        //        var result = responseTask;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = await result.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<ParticipantsModel>(readTask);
        //        }
        //    }

        //    return model;
        //}

        //public async Task<InvestorsModel> Investors()
        //{
        //    InvestorsModel model = new InvestorsModel();

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Configuration.GetConnectionString("investorUri"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //HTTP GET
        //        var responseTask = await client.GetAsync("investors/portfolio/" + _portfolioId);

        //        var result = responseTask;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = await result.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<InvestorsModel>(readTask);
        //        }
        //    }

        //    return model;
        //}

        //public async Task<ContractsModel> SearchContract(int id)
        //{
        //    ContractsModel model = new ContractsModel();

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Configuration.GetConnectionString("contractUri"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //HTTP GET
        //        var responseTask = await client.GetAsync("contracts/" + id + "/portfolio/" + _portfolioId);

        //        var result = responseTask;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = await result.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<ContractsModel>(readTask);
        //        }
        //    }

        //    return model;
        //}
    }
}
