using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMaP.Models;
using PMaP.Models.ViewModels.PortfolioValuation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public class PortfolioValuationService
    {
        private IConfiguration Configuration { get; }

        public PortfolioValuationService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<PortfolioValuationModel> Index(string portfolio, string subportfolio, string isAdd = "0")
        {
            PortfolioValuationModel model = new PortfolioValuationModel { ViewModel = new ViewModel { ExcludedContractIds = new List<int>() } };
            PrepareDropDownLists(ref model);

            PortfolioModel portfolioModel = new PortfolioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                var responseTask = await client.GetAsync("portfolios?portfolio=" + portfolio + "&subportfolio=" + subportfolio);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    portfolioModel = JsonConvert.DeserializeObject<PortfolioModel>(readTask);
                }
            }

            if (portfolioModel.ResponseCode == 200 && portfolioModel.Documents != null && portfolioModel.Documents.Count() > 0)
            {
                var portfolioContext = portfolioModel.Documents.First();
                portfolioContext.Id = isAdd == "1" ? portfolioContext.Id : 0;
                model.ViewModel.PortfolioValuationAdd = new PortfolioValuationAdd
                {
                    Portfolio = portfolioContext,
                    NoContracts = portfolioContext.ContractsNavigation.Count()
                };
            }

            model.ViewModel.IsAdd = isAdd;
            return model;
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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model.ViewModel), Encoding.UTF8, "application/json");
                //HTTP POST
                var responseTask = await client.PostAsync("portfolioValuation/summary", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            viewModel.DebtOB = debtOB;
            viewModel.DebtorType = debtorType;
            viewModel.DebtType = debtType;
            viewModel.Insolvency = insolvency;
            viewModel.Judicialized = judicialized;
            viewModel.PerformingStatus = performingStatus;
            viewModel.Region = region;
            model.ViewModel = viewModel;
            return model;
        }

        public async Task<PortfolioValuationModel> Details(PortfolioValuationModel model)
        {
            ViewModel viewModel = model.ViewModel;
            Summary summary = model.Summary;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("portfolioValuationUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model.ViewModel), Encoding.UTF8, "application/json");
                //HTTP POST
                var responseTask = await client.PostAsync("portfolioValuation/details/contracts", content);

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<PortfolioValuationModel>(readTask);
                }
            }

            model.Contracts.ForEach(x => x.Portfolio = null);
            if (viewModel.AddedInPortfolio == "1")
            {
                model.Contracts.ForEach(x => x.Portfolio = x.PortfolioContracts?.Where(x => x.Portfolio.OperationType == "SALE").OrderByDescending(x => x.Id).FirstOrDefault()?.Portfolio?.Portfolio1);
            }
            else if (string.IsNullOrEmpty(viewModel.AddedInPortfolio) || viewModel.AddedInPortfolio == "Select")
            {
                foreach (var item in model.Contracts)
                {
                    string portfolio = "";
                    portfolio = item.PortfolioContracts?.Where(x => x.Portfolio.OperationType == "SALE").OrderByDescending(x => x.Id).FirstOrDefault()?.Portfolio?.Portfolio1;
                    if (string.IsNullOrEmpty(portfolio))
                    {
                        portfolio = item.PortfolioContracts?.Where(x => x.Portfolio.OperationType == "DISCARD").OrderByDescending(x => x.Id).FirstOrDefault()?.Portfolio?.Portfolio1;
                    }
                    item.Portfolio = portfolio;
                }
            }
            else
            {
                model.Contracts.ForEach(x => x.Portfolio = x.PortfolioContracts?.Where(x => x.Portfolio.OperationType == "DISCARD").OrderByDescending(x => x.Id).FirstOrDefault()?.Portfolio?.Portfolio1);
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
                Text = "Select",
                Value = ""
            };

            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Secured", Value = "Secured" });
            selectListItems.Add(new SelectListItem { Text = "Unsecured", Value = "Unsecured" });
            model.ViewModel.DebtTypeList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Yes", Value = "1" });
            selectListItems.Add(new SelectListItem { Text = "No", Value = "0" });
            model.ViewModel.JudicializedList = model.ViewModel.InsolvencyList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Performing", Value = "PL" });
            selectListItems.Add(new SelectListItem { Text = "Sub-performing", Value = "SPL" });
            selectListItems.Add(new SelectListItem { Text = "Non-performing", Value = "NPL" });
            model.ViewModel.PerformingStatusList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Menor/igual de 1k €", Value = "1000" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 1k €", Value = "4999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 5k €", Value = "9999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 10k €", Value = "49999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 50k €", Value = "99999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 100k €", Value = "149999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 150k €", Value = "199999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 200k €", Value = "299999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 300k €", Value = "499999" });
            selectListItems.Add(new SelectListItem { Text = "Mayor de 500k €", Value = "500000" });
            model.ViewModel.DebtOBList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Particulares", Value = "TIT" });
            selectListItems.Add(new SelectListItem { Text = "Empresas", Value = "FIA" });
            model.ViewModel.DebtorTypeList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Norte", Value = "Norte" });
            selectListItems.Add(new SelectListItem { Text = "Noreste", Value = "Noreste" });
            selectListItems.Add(new SelectListItem { Text = "Noroeste", Value = "Noroeste" });
            selectListItems.Add(new SelectListItem { Text = "Centro", Value = "Centro" });
            selectListItems.Add(new SelectListItem { Text = "Centro-oeste", Value = "Centro-oeste" });
            selectListItems.Add(new SelectListItem { Text = "Centro-este", Value = "Centro-este" });
            selectListItems.Add(new SelectListItem { Text = "Sur", Value = "Sur" });
            selectListItems.Add(new SelectListItem { Text = "Sureste", Value = "Sureste" });
            selectListItems.Add(new SelectListItem { Text = "Suroeste", Value = "Suroeste" });
            model.ViewModel.RegionList = selectListItems;

            selectListItems = new List<SelectListItem>();
            selectListItems.Add(listItem);
            selectListItems.Add(new SelectListItem { Text = "Yes", Value = "1" });
            selectListItems.Add(new SelectListItem { Text = "No", Value = "0" });
            model.ViewModel.AddedInPortfolioList = selectListItems;

            List<ContractType> contractTypes = new List<ContractType>();
            contractTypes.Add(new ContractType { Id = 1, Name = "Préstamo Hipotecario" });
            contractTypes.Add(new ContractType { Id = 2, Name = "Descubiertos" });
            contractTypes.Add(new ContractType { Id = 3, Name = "Descuentos comerciales" });
            contractTypes.Add(new ContractType { Id = 4, Name = "Factoring" });
            contractTypes.Add(new ContractType { Id = 5, Name = "Préstamo Consumo" });
            contractTypes.Add(new ContractType { Id = 6, Name = "Tarjetas" });
            contractTypes.Add(new ContractType { Id = 7, Name = "Leasing mobiliario" });
            contractTypes.Add(new ContractType { Id = 8, Name = "Otros productos" });
            contractTypes.Add(new ContractType { Id = 9, Name = "Préstamo Empresas" });
            contractTypes.Add(new ContractType { Id = 10, Name = "Avales y garantias" });
            contractTypes.Add(new ContractType { Id = 11, Name = "Leasing inmobiliario" });
            contractTypes.Add(new ContractType { Id = 12, Name = "REOs" });
            contractTypes.Add(new ContractType { Id = 13, Name = "Créditos Empresas" });
            contractTypes.Add(new ContractType { Id = 14, Name = "Préstamo Particulares" });
            contractTypes.Add(new ContractType { Id = 15, Name = "Confirming" });
            model.ViewModel.ContractTypes = contractTypes;
        }
    }
}
