using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class InvestorsModel
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public IEnumerable<Investor> Documents { get; set; }
    }

    public class Investor
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public int? ContractId { get; set; }
        public string InvestorName { get; set; }
        public string SocialAddress { get; set; }
        public string TaxIdentification { get; set; }
        public string Mail { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Iban { get; set; }
        public string Bank { get; set; }
    }
}
