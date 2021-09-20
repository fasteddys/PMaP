using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.Portfolio;
using PMaP.Models.ViewModels.PortfolioValuation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class PortfolioModel
    {
        public ViewModels.Portfolio.ViewModel ViewModel { get; set; }

        public int ResponseCode { get; set; }
        public string Message { get; set; }

        public Summary Summary { get; set; }
        public List<Portfolio> Portfolios { get; set; }
        public List<Contract> Contracts { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
        public List<Procedure> Procedures { get; set; }

        public string ActiveTab { get; set; }

        [Required]
        public string Portfolio { get; set; }
    }
}
