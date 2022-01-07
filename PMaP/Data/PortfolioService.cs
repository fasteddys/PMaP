using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PMaP.Models;
using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.Portfolio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
            try
            {
                var model = await _httpService.Get<IEnumerable<Portfolio>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio/GetAllQuery" + queryStrings) ?? new List<Portfolio>();

                List<SelectListItem> portfolioList = new List<SelectListItem>();
                portfolioList.Add(new SelectListItem() { Text = "Select", Value = "" });
                List<SelectListItem> subportfolioList = new List<SelectListItem>();
                subportfolioList.Add(new SelectListItem() { Text = "Select", Value = "" });
                if (model != null && model.Count() > 0)
                {
                    var portfolios = model.GroupBy(x => x.Portfolio1).Select(x => x.First()).OrderBy(x => x.Portfolio1).ToList();
                    foreach (var item in portfolios)
                    {
                        portfolioList.Add(new SelectListItem() { Text = item.Portfolio1, Value = item.Portfolio1 });
                    }

                }

                return new PortfolioModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    ViewModel = new ViewModel
                    {
                        PortfolioList = portfolioList,
                        SubportfolioList = subportfolioList
                    },
                    Portfolios = model.ToList()
                };
            }
            catch (Exception ex)
            {
                return new PortfolioModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        public async Task<PortfolioModel> Characteristics(PortfolioModel model)
        {
            try
            {
                PortfolioModel response = new PortfolioModel();
                _portfolioId = 0;
                ViewModel viewModel = model.ViewModel;

                string portfolioViewModel = model.ViewModel.PortfolioList.Find(x => x.Text == model.ViewModel.Portfolio)?.Value;
                string subportfolio = model.ViewModel.SubportfolioList.Find(x => x.Text == model.ViewModel.Subportfolio)?.Value;
                var httpResponse = await _httpService.Get<IEnumerable<Portfolio>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio/GetAllQuery?portfolio=" + (portfolioViewModel?.ToLower() != "select" ? portfolioViewModel : "") + "&subportfolio=" + (subportfolio?.ToLower() != "select" ? subportfolio : ""));
                if (httpResponse != null)
                {
                    response.ViewModel = viewModel;

                    response.ViewModel.OperationType = response.ViewModel.OBCutOff = response.ViewModel.OBSingning = response.ViewModel.OBClosing = "";
                    response.ViewModel.DateAdded = response.ViewModel.DateCutOff = response.ViewModel.DateSigning = response.ViewModel.DateClosing = null;
                    if (httpResponse != null && httpResponse.Count() > 0)
                    {
                        var portfolio = httpResponse.First();
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

                    response.ResponseCode = (int)HttpStatusCode.OK;
                    response.Portfolios = httpResponse.ToList();
                    return response;
                }

                model.ViewModel = viewModel;

                model.ViewModel.OperationType = model.ViewModel.OBCutOff = model.ViewModel.OBSingning = model.ViewModel.OBClosing = "";
                model.ViewModel.DateAdded = model.ViewModel.DateCutOff = model.ViewModel.DateSigning = model.ViewModel.DateClosing = null;
                if (httpResponse != null && httpResponse != null && httpResponse.Count() > 0)
                {
                    var portfolio = httpResponse.First();
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

                model.ResponseCode = (int)HttpStatusCode.OK;
                return model;
            }
            catch (Exception ex)
            {
                return new PortfolioModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        public async Task<ContractsModel> Assessment()
        {
            try
            {
                ContractsModel model = new ContractsModel { Summary = new Summary() };

                var portfolio = await _httpService.Get<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio/" + _portfolioId);
                if (portfolio != null)
                {
                    return new ContractsModel
                    {
                        ResponseCode = (int)HttpStatusCode.OK,
                        Summary = new Summary
                        {
                            Contracts = portfolio.ContractsNavigation.Count(),
                            Debtors = portfolio.ContractsNavigation.Sum(x => x.NumParticipants ?? 0),
                            Guarantors = portfolio.ContractsNavigation.Sum(x => x.NumGuarantors ?? 0),
                            SecuredOB = 0,
                            SecuredPrice = 0,
                            TotalOB = portfolio.ContractsNavigation.Sum(x => x.TotalAmountOb ?? 0),
                            UnsecuredOB = 0,
                            UnsecuredPrice = 0
                        }
                    };
                }

                return model;
            }
            catch (Exception ex)
            {
                return new ContractsModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message, Summary = new Summary() };
            }
        }

        public async Task<ContractsModel> Contracts()
        {
            try
            {
                var portfolio = await _httpService.Get<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio/" + _portfolioId)
                    ?? new Portfolio
                    {
                        ContractsNavigation = new List<Contract>(),
                        Investors = new List<Investor>(),
                        Participants = new List<Participant>(),
                        Procedures = new List<Procedure>()
                    };

                var contracts = portfolio.ContractsNavigation?.ToList();

                var investors = new List<Investor>();
                var participants = new List<Participant>();
                var procedures = new List<Procedure>();
                var collaterals = new List<Collateral>();

                foreach (var contract in contracts)
                {
                    investors.AddRange(contract.Investors);
                    participants.AddRange(contract.Participants);
                    procedures.AddRange(contract.Procedures);
                    collaterals.AddRange(contract.Collaterals);
                }

                return new ContractsModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    Contracts = contracts,
                    Investors = investors,
                    Participants = participants,
                    Procedures = procedures,
                    Collaterals = collaterals
                };
            }
            catch (Exception ex)
            {
                return new ContractsModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
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
            try
            {
                var portfolio = await _httpService.Get<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio/" + _portfolioId);
                
                var contracts = portfolio.ContractsNavigation?.ToList();

                var investors = new List<Investor>();
                var participants = new List<Participant>();
                var procedures = new List<Procedure>();
                var collaterals = new List<Collateral>();

                foreach (var contract in contracts)
                {
                    investors.AddRange(contract.Investors);
                    participants.AddRange(contract.Participants);
                    procedures.AddRange(contract.Procedures);
                    collaterals.AddRange(contract.Collaterals);
                }

                return new ContractsModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    Contracts = contracts,
                    Investors = investors,
                    Participants = participants,
                    Procedures = procedures,
                    Collaterals = collaterals
                };
            }
            catch (Exception ex)
            {
                return new ContractsModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
    }
}
