using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.PortfolioValuation;
using System.Collections.Generic;

namespace PMaP.Models
{
    public class PortfolioValuationModel
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public Summary Summary { get; set; }
        public List<Contract> Contracts { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
        public List<Procedure> Procedures { get; set; }

        public ViewModel ViewModel { get; set; }
    }
}
