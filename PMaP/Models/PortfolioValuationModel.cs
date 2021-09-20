using PMaP.Models.DBModels;
using PMaP.Models.ViewModels.PortfolioValuation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class Summary
    {
        public int Contracts { get; set; }
        public decimal TotalOB { get; set; }
        public decimal SecuredOB { get; set; }
        public decimal UnsecuredOB { get; set; }
        public decimal SecuredPrice { get; set; }
        public decimal UnsecuredPrice { get; set; }
        public int Debtors { get; set; }
        public int Guarantors { get; set; }
    }
}
