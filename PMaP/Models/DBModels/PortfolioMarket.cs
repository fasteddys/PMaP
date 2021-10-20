using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models.DBModels
{
    public partial class PortfolioMarket
    {
        public int Id { get; set; }
        public string Holder { get; set; }
        public string HolderLogo { get; set; }
        public string Project { get; set; }
        public string Investor { get; set; }
        public string InvestorLogo { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string Typology { get; set; }
        public string DebtType { get; set; }
        public decimal? Value { get; set; }
        public byte? Status { get; set; }

        public virtual ICollection<Contract> ContractsNavigation { get; set; }
        public virtual ICollection<Home> Homes { get; set; }
        public virtual ICollection<Investor> Investors { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<PortfolioContract> PortfolioContracts { get; set; }
        public virtual ICollection<Procedure> Procedures { get; set; }
    }
}
