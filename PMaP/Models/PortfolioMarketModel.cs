using PMaP.Models.DBModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMaP.Models
{
    public class PortfolioMarketModel
    {
        public ViewModels.Portfolio.ViewModel ViewModel { get; set; }

        public int ResponseCode { get; set; }
        public string Message { get; set; }

        public Summary Summary { get; set; }
        public List<PortfolioMarket> PortfolioMarkets { get; set; }
        public List<Contract> Contracts { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
        //public List<Procedure> Procedures { get; set; }

        public string ActiveTab { get; set; }

        [Required]
        public string PortfolioMarket { get; set; }
    }
}
