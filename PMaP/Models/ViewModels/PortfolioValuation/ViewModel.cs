using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace PMaP.Models.ViewModels.PortfolioValuation
{
    public class ViewModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string DebtType { get; set; }
        public List<SelectListItem> DebtTypeList { get; set; }
        public string Judicialized { get; set; }
        public List<SelectListItem> JudicializedList { get; set; }
        public string Insolvency { get; set; }
        public List<SelectListItem> InsolvencyList { get; set; }
        public string PerformingStatus { get; set; }
        public List<SelectListItem> PerformingStatusList { get; set; }
        public string DebtOB { get; set; }
        public List<SelectListItem> DebtOBList { get; set; }
        public string DebtorName { get; set; }
        public string DebtorType { get; set; }
        public List<SelectListItem> DebtorTypeList { get; set; }
        public string Region { get; set; }
        public List<SelectListItem> RegionList { get; set; }
        public List<ContractType> ContractTypes { get; set; }
        public PortfolioValuationAdd PortfolioValuationAdd { get; set; }
        public string IsAdd { get; set; }
        public List<int> ExcludedContractIds { get; set; }
        public string AddedInPortfolio { get; set; }
        public List<SelectListItem> AddedInPortfolioList { get; set; }
        public bool ReflectExcludedContractIds { get; set; }
        public bool ExcludePossitiveOB { get; set; }
    }

    public class ContractType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string Value { get; set; }
    }

    public class PortfolioValuationAdd
    {
        //public int PortfolioId { get; set; }
        //public string Portfolio { get; set; }
        //public string Subportfolio { get; set; }
        public DBModels.Portfolio Portfolio { get; set; }
        public string Situation { get; set; }
        //public string OB { get; set; }
        public int NoContracts { get; set; }
    }
}
