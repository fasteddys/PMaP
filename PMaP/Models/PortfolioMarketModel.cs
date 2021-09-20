using PMaP.Models.ViewModels.Portfolio;
using PMaP.Models.ViewModels.PortfolioValuation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class PortfolioMarketModel
    {
        public ViewModels.Portfolio.ViewModel ViewModel { get; set; }

        public int ResponseCode { get; set; }
        public string Message { get; set; }

        public Summary Summary { get; set; }
        public List<PortfolioMarket> Documents { get; set; }
        public List<Contract> Contracts { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
        //public List<Procedure> Procedures { get; set; }

        public string ActiveTab { get; set; }

        [Required]
        public string PortfolioMarket { get; set; }
    }

    public class PortfolioMarket
    {

        public int Id { get; set; }
        public string Holder { get; set; }
        public string Project { get; set; }
        public string Investor { get; set; }
        public string year { get; set; }
        public string Typology { get; set; }
        public string DebtType { get; set; }
        public string Value { get; set; }
        public byte? Status { get; set; }

        public virtual ICollection<Contract> ContractsNavigation { get; set; }
        public virtual ICollection<Home> Homes { get; set; }
        public virtual ICollection<Investor> Investors { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
    }
}
