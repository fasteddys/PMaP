using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using PMaP.Models;
using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.PortfolioValuation;
using PMaP.Pages.PortfolioEvaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IPortfolioValuationService
    {
        Task<PortfolioValuationModel> Index(string portfolio, string subportfolio, string isAdd = "0");
        Task<PortfolioValuationModel> Summary(PortfolioValuationModel model);
        Task<PortfolioValuationModel> Details(PortfolioValuationModel model);
    }

    public class PortfolioValuationService : IPortfolioValuationService
    {
        private IConfiguration Configuration { get; }
        private IHttpService _httpService;
        private IStringLocalizer<PortfolioValuation> _localizer;

        public PortfolioValuationService(IConfiguration configuration, IHttpService httpService, IStringLocalizer<PortfolioValuation> localizer)
        {
            Configuration = configuration;
            _httpService = httpService;
            _localizer = localizer;
        }

        public async Task<PortfolioValuationModel> Index(string portfolio, string subportfolio, string isAdd = "0")
        {
            PortfolioValuationModel model = new PortfolioValuationModel { ViewModel = new ViewModel { ExcludedContractIds = new List<int>() } };
            try
            {
                PrepareDropDownLists(ref model);

                var portfolios = await _httpService.Get<IEnumerable<Portfolio>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Portfolio/GetAllQuery?portfolio=" + portfolio + "&subportfolio=" + subportfolio);

                model.ViewModel.PortfolioValuationAdd = new PortfolioValuationAdd { Portfolio = new Portfolio() };

                if (portfolios != null && portfolios.Count() > 0)
                {
                    var portfolioContext = portfolios.First();
                    portfolioContext.Id = isAdd == "1" ? portfolioContext.Id : 0;
                    model.ViewModel.PortfolioValuationAdd = new PortfolioValuationAdd
                    {
                        Portfolio = portfolioContext,
                        NoContracts = portfolioContext.ContractsNavigation.Count()
                    };
                }

                model.ViewModel.IsAdd = isAdd;
                model.ResponseCode = (int)HttpStatusCode.OK;
                return model;
            }
            catch (Exception ex)
            {
                model.ResponseCode = (int)HttpStatusCode.InternalServerError;
                model.Message = ex.Message;
                return model;
            }
        }

        public async Task<PortfolioValuationModel> Summary(PortfolioValuationModel model)
        {
            ViewModel viewModel = model.ViewModel;

            string debtOB = model.ViewModel.DebtOB, debtorType = model.ViewModel.DebtorType, debtType = model.ViewModel.DebtType,
                insolvency = model.ViewModel.Insolvency, judicialized = model.ViewModel.Judicialized,
                performingStatus = model.ViewModel.PerformingStatus, region = model.ViewModel.Region;

            model.ViewModel.DebtOB = model.ViewModel.DebtOB != "Select" ? model.ViewModel.DebtOB : model.ViewModel.DebtOBList.Find(x => x.Text == model.ViewModel.DebtOB)?.Value;
            model.ViewModel.DebtorType = model.ViewModel.DebtorType != "Select" ? model.ViewModel.DebtorType : model.ViewModel.DebtorTypeList.Find(x => x.Text == model.ViewModel.DebtorType)?.Value;
            model.ViewModel.DebtType = model.ViewModel.DebtType != "Select" ? model.ViewModel.DebtType : model.ViewModel.DebtTypeList.Find(x => x.Text == model.ViewModel.DebtType)?.Value;
            model.ViewModel.Insolvency = model.ViewModel.Insolvency != "Select" ? model.ViewModel.Insolvency : model.ViewModel.InsolvencyList.Find(x => x.Text == model.ViewModel.Insolvency)?.Value;
            model.ViewModel.Judicialized = model.ViewModel.Judicialized != "Select" ? model.ViewModel.Judicialized : model.ViewModel.JudicializedList.Find(x => x.Text == model.ViewModel.Judicialized)?.Value;
            model.ViewModel.PerformingStatus = model.ViewModel.PerformingStatus != "Select" ? model.ViewModel.PerformingStatus : model.ViewModel.PerformingStatusList.Find(x => x.Text == model.ViewModel.PerformingStatus)?.Value;
            model.ViewModel.Region = model.ViewModel.Region != "Select" ? model.ViewModel.Region : model.ViewModel.RegionList.Find(x => x.Text == model.ViewModel.Region)?.Value;

            var contracts = await _httpService.Post<IEnumerable<Contract>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Contract/GetAllQuery", model.ViewModel);
            
            viewModel.DebtOB = debtOB;
            viewModel.DebtorType = debtorType;
            viewModel.DebtType = debtType;
            viewModel.Insolvency = insolvency;
            viewModel.Judicialized = judicialized;
            viewModel.PerformingStatus = performingStatus;
            viewModel.Region = region;
            model.ViewModel = viewModel;

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

                return new PortfolioValuationModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    Contracts = contracts.ToList(),
                    Investors = investors,
                    Participants = participants,
                    Procedures = procedures,
                    ViewModel = viewModel,
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
                    }
                };
            }

            return model;
        }

        public async Task<PortfolioValuationModel> Details(PortfolioValuationModel model)
        {
            ViewModel viewModel = model.ViewModel;
            Summary summary = model.Summary;

            var contracts = await _httpService.Post<IEnumerable<Contract>>(Configuration.GetConnectionString("pmapApiUrl") + "/api/Contract/GetAllQuery", model.ViewModel);

            if (contracts != null && contracts.Count() > 0)
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

                return new PortfolioValuationModel
                {
                    ResponseCode = (int)HttpStatusCode.OK,
                    Contracts = contracts.ToList(),
                    Investors = investors,
                    Participants = participants,
                    Procedures = procedures,
                    ViewModel = viewModel,
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
                    }
                };
            }

            model.ViewModel = viewModel;
            model.Summary = summary;
            return model;
        }

        private void PrepareDropDownLists(ref PortfolioValuationModel model)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            SelectListItem listItem = new SelectListItem
            {
                Text = _localizer["Select"],
                Value = ""
            };

            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["Secured"], Value = "Secured" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Unsecured"], Value = "Unsecured" });
            model.ViewModel.DebtTypeList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["Yes"], Value = "1" });
            selectListItems.Add(new SelectListItem { Text = _localizer["No"], Value = "0" });
            model.ViewModel.JudicializedList = model.ViewModel.InsolvencyList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["Performing"], Value = "PL" });
            selectListItems.Add(new SelectListItem { Text = _localizer["SubPerforming"], Value = "SPL" });
            selectListItems.Add(new SelectListItem { Text = _localizer["NonPerforming"], Value = "NPL" });
            model.ViewModel.PerformingStatusList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["MenorIgualDe1k"], Value = "1000" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe1k"], Value = "4999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe5k"], Value = "9999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe10k"], Value = "49999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe50k"], Value = "99999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe100k"], Value = "149999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe150k"], Value = "199999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe200k"], Value = "299999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe300k"], Value = "499999" });
            selectListItems.Add(new SelectListItem { Text = _localizer["MayorDe500k"], Value = "500000" });
            model.ViewModel.DebtOBList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["Particulares"], Value = "TIT" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Empresas"], Value = "FIA" });
            model.ViewModel.DebtorTypeList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["Norte"], Value = "Norte" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Noreste"], Value = "Noreste" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Noroeste"], Value = "Noroeste" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Centro"], Value = "Centro" });
            selectListItems.Add(new SelectListItem { Text = _localizer["CentroOeste"], Value = "Centro-oeste" });
            selectListItems.Add(new SelectListItem { Text = _localizer["CentroEste"], Value = "Centro-este" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Sur"], Value = "Sur" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Sureste"], Value = "Sureste" });
            selectListItems.Add(new SelectListItem { Text = _localizer["Suroeste"], Value = "Suroeste" });
            model.ViewModel.RegionList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = _localizer["Yes"], Value = "1" });
            selectListItems.Add(new SelectListItem { Text = _localizer["No"], Value = "0" });
            model.ViewModel.AddedInPortfolioList = selectListItems;

            List<ContractType> contractTypes = new List<ContractType>();
            contractTypes.Add(new ContractType { Id = 1, Name = _localizer["PrestamoHipotecario"], Value = "PH" });
            contractTypes.Add(new ContractType { Id = 2, Name = _localizer["ContratoDeHipotecaDeMaximo"], Value = "CDHDM" });
            contractTypes.Add(new ContractType { Id = 3, Name = _localizer["CreditoConGarantiaHipotecaria"], Value = "CCGH" });
            contractTypes.Add(new ContractType { Id = 4, Name = _localizer["Prestamo"], Value = "P" });
            contractTypes.Add(new ContractType { Id = 5, Name = _localizer["PrestamoConsumo"], Value = "PC" });
            contractTypes.Add(new ContractType { Id = 6, Name = _localizer["Tarjetas"], Value = "T" });
            contractTypes.Add(new ContractType { Id = 7, Name = _localizer["LeasingMobiliario"], Value = "LM" });
            contractTypes.Add(new ContractType { Id = 8, Name = _localizer["OtrosProductos"], Value = "OP" });
            contractTypes.Add(new ContractType { Id = 9, Name = _localizer["PrestamoEmpresas"], Value = "PE" });
            contractTypes.Add(new ContractType { Id = 10, Name = _localizer["AvalesYGarantias"], Value = "AYG" });
            contractTypes.Add(new ContractType { Id = 11, Name = _localizer["LeasingInmobiliario"], Value = "LI" });
            contractTypes.Add(new ContractType { Id = 12, Name = _localizer["REOs"], Value = "R" });
            contractTypes.Add(new ContractType { Id = 13, Name = _localizer["CreditosEmpresas"], Value = "CE" });
            contractTypes.Add(new ContractType { Id = 14, Name = _localizer["PrestamoParticulares"], Value = "PP" });
            contractTypes.Add(new ContractType { Id = 15, Name = _localizer["Confirming"], Value = "C" });
            model.ViewModel.ContractTypes = contractTypes;
        }
    }
}
