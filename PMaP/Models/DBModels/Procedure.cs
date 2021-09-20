using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models.DBModels
{
    public partial class Procedure
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public string Portfolio { get; set; }
        public string Subportfolio { get; set; }
        public DateTime? ProcessDate { get; set; }
        public int? ContractId { get; set; }
        public string Contract { get; set; }
        public string ProcedureId { get; set; }
        public string ProceedingId { get; set; }
        public string ProceedingType { get; set; }
        public string ProcedureType { get; set; }
        public DateTime? LawsuitDate { get; set; }
        public string LastJudicialPhaseDescription { get; set; }
        public DateTime? LastJudicialPhaseDate { get; set; }
        public decimal? AmountClaimed { get; set; }
        public decimal? JudicialCostAmount { get; set; }
        public string CourtProcedureNumber { get; set; }
        public string Court { get; set; }
        public string CourtNumber { get; set; }
        public string CourtCity { get; set; }
        public string CourtProvince { get; set; }
        public string LaywerName { get; set; }
        public string SolicitorName { get; set; }
        public string LaywerMail { get; set; }
        public string SolicitorMail { get; set; }
        public string LaywerPhoneNumber { get; set; }
        public string SolicitorPhoneNumber { get; set; }
        public string LawFirm { get; set; }

        public virtual Contract ContractNavigation { get; set; }
        public virtual Portfolio PortfolioNavigation { get; set; }
    }
}
