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
        private IHttpService _httpService;

        public PortfolioRegistrationService(IConfiguration configuration, IHttpService httpService)
        {
            Configuration = configuration;
            _httpService = httpService;
        }

        public async Task<PortfolioModel> Index(Models.ViewModels.PortfolioValuation.ViewModel portfolioValuationViewModel)
        {
            if (portfolioValuationViewModel != null && portfolioValuationViewModel.PortfolioValuationAdd != null)
            {
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 = "";
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio = "";
            }

            PortfolioValuationModel portfolioValuationModel = new PortfolioValuationModel();
            
            var contracts = await _httpService.Post<IEnumerable<Contract>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Contract/GetAllQuery", portfolioValuationViewModel);
            if (contracts != null)
            {
                var investors = new List<Investor>();
                var participants = new List<Participant>();
                var procedures = new List<Procedure>();

                foreach (var contract in contracts)
                {
                    investors.AddRange(contract.Investors);
                    participants.AddRange(contract.Participants);
                    procedures.AddRange(contract.Procedures);
                }

                PortfolioModel portfolioModelResponse = new PortfolioModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    ActiveTab = "summary",
                    Contracts = contracts.ToList(),
                    Participants = participants,
                    Investors = investors,
                    Procedures = procedures,
                    Summary = new Summary
                    {
                        Contracts = contracts.Count(),
                        Debtors = contracts.Sum(x => x.NumParticipants ?? 0),
                        Guarantors = contracts.Sum(x => x.NumGuarantors ?? 0),
                        SecuredOB = 0,
                        SecuredPrice = 0,
                        TotalOB = contracts.Sum(x => x.TotalAmountOb ?? 0),
                        UnsecuredOB = 0,
                        UnsecuredPrice = 0
                    },
                    ViewModel = new ViewModel
                    {
                        DateAdded = DateTime.Now,
                        PortfolioId = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Id : 0,
                        Portfolio = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 : "",
                        Subportfolio = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio : ""
                    }
                };

                return portfolioModelResponse;
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
                    PortfolioId = portfolioValuationViewModel != null && portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Id : 0,
                    Portfolio = portfolioValuationViewModel != null && portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 : "",
                    Subportfolio = portfolioValuationViewModel != null && portfolioValuationViewModel.PortfolioValuationAdd != null? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio : ""
                }
            };

            return portfolioModel;
        }

        public async Task<PortfolioValuationModel> AddPortfolio(PortfolioModel model)
        {
            try
            {
                Portfolio portfolio = new Portfolio
                {
                    Portfolio1 = model.ViewModel.Portfolio,
                    Subportfolio = model.ViewModel.Subportfolio,
                    OperationType = "SALE",
                    ClosingDate = model.ViewModel.DateClosing,
                    CreationDate = model.ViewModel.DateAdded,
                    CutOffDate = model.ViewModel.DateCutOff,
                    SigningDate = model.ViewModel.DateSigning
                };
                var portfolioResp = await _httpService.Post<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio", portfolio);

                model.Contracts.ForEach(x =>
                {
                    x.Investors = null;
                    x.Participants = null;
                    x.PortfolioNavigation = null;
                    x.Procedures = null;
                });
                portfolioResp.ContractsNavigation = model.Contracts;
                await _httpService.Put<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio", portfolioResp);

                return new PortfolioValuationModel { ResponseCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new PortfolioValuationModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        public async Task<PortfolioValuationModel> UpdatePortfolio(PortfolioValuationModel model)
        {
            try
            {
                Portfolio portfolio = model.ViewModel.PortfolioValuationAdd.Portfolio;
                model.Contracts.ForEach(x =>
                {
                    x.Investors = null;
                    x.Participants = null;
                    x.PortfolioNavigation = null;
                    x.Procedures = null;
                });
                portfolio.ContractsNavigation = model.Contracts;
                await _httpService.Put<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio", portfolio);
                return new PortfolioValuationModel { ResponseCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new PortfolioValuationModel { ResponseCode= (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        public async Task<PortfolioValuationModel> DiscardPortfolio(Portfolio portfolio)
        {
            try
            {
                portfolio.OperationType = "DISCARD";
                await _httpService.Put<Portfolio>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio", portfolio);
                return new PortfolioValuationModel { ResponseCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new PortfolioValuationModel { ResponseCode= (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        public async Task<PortfolioValuationModel> DeletePortfolioContracts(int portfolioId, List<Contract> contracts)
        {
            try
            {
                foreach (var contract in contracts)
                {
                    contract.PortfolioId = null;
                    await _httpService.Put<Contract>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Contract", contract);
                }
                return new PortfolioValuationModel { ResponseCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new PortfolioValuationModel { ResponseCode= (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
    }
}
