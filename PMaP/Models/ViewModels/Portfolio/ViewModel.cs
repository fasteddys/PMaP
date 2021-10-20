using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models.ViewModels.Portfolio
{
    public class ViewModel
    {
        public int PortfolioId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Portfolio is required.")]
        public string Portfolio { get; set; }
        public List<SelectListItem> PortfolioList { get; set; }
        public string Subportfolio { get; set; }
        public List<SelectListItem> SubportfolioList { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateCutOff { get; set; }
        public DateTime? DateSigning { get; set; }
        public DateTime? DateClosing { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public decimal? ValueFrom { get; set; }
        public decimal? ValueTo { get; set; }

        public string OperationType { get; set; }
        public string Situation { get; set; }
        public string OBCutOff { get; set; }
        public string OBSingning { get; set; }
        public string OBClosing { get; set; }
        public int? Contract { get; set; }
        public string Holder { get; set; }

        public PortfolioValuationAdd PortfolioValuationAdd { get; set; }
    }

    public class PortfolioValuationAdd
    {
        public string Portfolio { get; set; }
        public string Subportfolio { get; set; }
        public string Situation { get; set; }
        public decimal OB { get; set; }
        public int NoContracts { get; set; }
    }
}
