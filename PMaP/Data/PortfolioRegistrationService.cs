using Microsoft.Extensions.Configuration;
using PMaP.Models;
using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.Portfolio;
using System;
using System.Collections.Generic;
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
            if (portfolioValuationViewModel.PortfolioValuationAdd != null)
            {
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 = "";
                portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio = "";
            }

            PortfolioValuationModel portfolioValuationModel = new PortfolioValuationModel();
            
            var response = await _httpService.Post<PortfolioValuationModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolioEvaluation/details/contracts", portfolioValuationViewModel);
            if (response != null)
            {
                PortfolioModel portfolioModelResponse = new PortfolioModel
                {
                    ResponseCode = response.ResponseCode,
                    ActiveTab = "summary",
                    Contracts = response.Contracts,
                    Participants = response.Participants,
                    Investors = response.Investors,
                    Procedures = response.Procedures,
                    Summary = response.Summary,
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
                    PortfolioId = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Id : 0,
                    Portfolio = portfolioValuationViewModel.PortfolioValuationAdd != null ? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Portfolio1 : "",
                    Subportfolio = portfolioValuationViewModel.PortfolioValuationAdd != null? portfolioValuationViewModel.PortfolioValuationAdd.Portfolio.Subportfolio : ""
                }
            };

            return portfolioModel;
        }

        public async Task<PortfolioValuationModel> AddPortfolio(PortfolioModel model)
        {
            return await _httpService.Post<PortfolioValuationModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolioEvaluation/portfolio", model) ?? new PortfolioValuationModel();
        }

        public async Task<PortfolioValuationModel> UpdatePortfolio(PortfolioValuationModel model)
        {
            return await _httpService.Put<PortfolioValuationModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolioEvaluation/portfolio", model) ?? new PortfolioValuationModel() ?? new PortfolioValuationModel();
        }

        public async Task<PortfolioValuationModel> DiscardPortfolio(Portfolio portfolio)
        {
            return await _httpService.Delete<PortfolioValuationModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolioEvaluation/portfolio/" + portfolio.Id) ?? new PortfolioValuationModel();
        }

        public async Task<PortfolioValuationModel> DeletePortfolioContracts(int portfolioId, List<Contract> contracts)
        {
            return await _httpService.Put<PortfolioValuationModel>(Configuration.GetConnectionString("pmapApiUrl") + "/api/portfolioEvaluation/portfolio/" + portfolioId + "/contracts", contracts) ?? new PortfolioValuationModel();
        }
    }
}
