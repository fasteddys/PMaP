using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PMaP.Models;
using PMaP.Models.ViewModels.Portfolio;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IPortfolioService
    {
        Task<PortfolioModel> Portfolios(string queryStrings);
        Task<PortfolioModel> Characteristics(PortfolioModel model);
        Task<ContractsModel> Assessment();
        Task<ContractsModel> Contracts();
        Task<ContractsModel> SearchContract(int id);
    }

    public class PortfolioService : IPortfolioService
    {
        private IConfiguration Configuration { get; }
        private IHttpService _httpService;
        public static int _portfolioId;

        public PortfolioService(IConfiguration configuration, IHttpService httpService)
        {
            Configuration = configuration;
            _httpService = httpService;
        }

        public async Task<PortfolioModel> Portfolios(string queryStrings)
        {
            PortfolioModel model = new PortfolioModel();

            model = await _httpService.Get<PortfolioModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolio" + queryStrings) ?? new PortfolioModel();

            List<SelectListItem> portfolioList = new List<SelectListItem>();
            portfolioList.Add(new SelectListItem() { Text = "Select", Value = "" });
            List<SelectListItem> subportfolioList = new List<SelectListItem>();
            subportfolioList.Add(new SelectListItem() { Text = "Select", Value = "" });
            if (model.ResponseCode == 200 && model.Portfolios != null && model.Portfolios.Count() > 0)
            {
                var portfolios = model.Portfolios.GroupBy(x => x.Portfolio1).Select(x => x.First()).OrderBy(x => x.Portfolio1).ToList();
                foreach (var item in portfolios)
                {
                    portfolioList.Add(new SelectListItem() { Text = item.Portfolio1, Value = item.Portfolio1 });
                }

            }

            return new PortfolioModel
            {
                ResponseCode = model.ResponseCode,
                ViewModel = new ViewModel
                {
                    PortfolioList = portfolioList,
                    SubportfolioList = subportfolioList
                },
                Portfolios = model.Portfolios
            };
        }

        public async Task<PortfolioModel> Characteristics(PortfolioModel model)
        {
            _portfolioId = 0;
            ViewModel viewModel = model.ViewModel;
            
            string portfolioViewModel = model.ViewModel.PortfolioList.Find(x => x.Text == model.ViewModel.Portfolio)?.Value;
            string subportfolio = model.ViewModel.SubportfolioList.Find(x => x.Text == model.ViewModel.Subportfolio)?.Value;
            var response = await _httpService.Get<PortfolioModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolio?portfolio=" + (portfolioViewModel?.ToLower() != "select" ? portfolioViewModel : "") + "&subportfolio=" + (subportfolio?.ToLower() != "select" ? subportfolio : ""));
            if (response != null)
            {
                response.ViewModel = viewModel;

                response.ViewModel.OperationType = response.ViewModel.OBCutOff = response.ViewModel.OBSingning = response.ViewModel.OBClosing = "";
                response.ViewModel.DateAdded = response.ViewModel.DateCutOff = response.ViewModel.DateSigning = response.ViewModel.DateClosing = null;
                if (response.ResponseCode == 200 && response.Portfolios != null && response.Portfolios.Count() > 0)
                {
                    var portfolio = response.Portfolios.First();
                    response.ViewModel.OperationType = portfolio.OperationType;
                    response.ViewModel.OBCutOff = portfolio.CutOffOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
                    response.ViewModel.OBSingning = portfolio.SigningOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
                    response.ViewModel.OBClosing = portfolio.ClosingOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
                    response.ViewModel.DateAdded = portfolio.CreationDate;
                    response.ViewModel.DateCutOff = portfolio.CutOffDate;
                    response.ViewModel.DateSigning = portfolio.SigningDate;
                    response.ViewModel.DateClosing = portfolio.ClosingDate;
                    _portfolioId = portfolio.Id;
                }

                return response;
            }
            
            model.ViewModel = viewModel;

            model.ViewModel.OperationType = model.ViewModel.OBCutOff = model.ViewModel.OBSingning = model.ViewModel.OBClosing = "";
            model.ViewModel.DateAdded = model.ViewModel.DateCutOff = model.ViewModel.DateSigning = model.ViewModel.DateClosing = null;
            if (response != null && response.ResponseCode == 200 && response.Portfolios != null && response.Portfolios.Count() > 0)
            {
                var portfolio = response.Portfolios.First();
                model.ViewModel.OperationType = portfolio.OperationType;
                model.ViewModel.OBCutOff = portfolio.CutOffOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
                model.ViewModel.OBSingning = portfolio.SigningOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
                model.ViewModel.OBClosing = portfolio.ClosingOb?.ToString("c", CultureInfo.CreateSpecificCulture("es-ES"));
                model.ViewModel.DateAdded = portfolio.CreationDate;
                model.ViewModel.DateCutOff = portfolio.CutOffDate;
                model.ViewModel.DateSigning = portfolio.SigningDate;
                model.ViewModel.DateClosing = portfolio.ClosingDate;
                _portfolioId = portfolio.Id;
            }

            return model;
        }

        public async Task<ContractsModel> Assessment()
        {
            ContractsModel model = new ContractsModel { Summary = new Summary() };

            var response = await _httpService.Get<ContractsModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/contract/portfolio/" + _portfolioId + "/assessment");
            if (response != null)
            {
                return response;
            }

            return model;
        }

        public async Task<ContractsModel> Contracts()
        {
            return await _httpService.Get<ContractsModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/contract/portfolio/" + _portfolioId) ?? new ContractsModel();
        }

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

        public async Task<ContractsModel> SearchContract(int id)
        {
            return await _httpService.Get<ContractsModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/contract/" + id + "/portfolio/" + _portfolioId) ?? new ContractsModel();
        }
    }
}
