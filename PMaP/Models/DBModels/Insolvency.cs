using System;

namespace PMaP.Models.DBModels
{
    public class Insolvency
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public string Portfolio { get; set; }
        public string Subportfolio { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string InsolvencyId { get; set; }
        public int? ParticipantId { get; set; }
        public string Participant { get; set; }
        public string InsolvencyType { get; set; }
        public DateTime? InsolvencyDate { get; set; }
        public string InsolvencyPhase { get; set; }
        public DateTime? LastInsolvencyActionDate { get; set; }
        public DateTime? InsolvencyAutoAdjudicationDate { get; set; }
        public decimal? RecognizedInclusionAmount { get; set; }
        public decimal? RecognizedInclusionUnderAmount { get; set; }
        public string CourtInsolvencyName { get; set; }
        public string CourtInsolvencyNumber { get; set; }
        public string CourtInsolvencyAddress { get; set; }
        public string CourtInsolvencyCity { get; set; }
        public string CourtInsolvencyProvince { get; set; }
        public string CourtInsolvencyZipCode { get; set; }
        public string CourtProcedureNumber { get; set; }
        public string InsolvencyManagerName { get; set; }
        public string InsolvencyManagerEmail { get; set; }
        public string InsolvencyManagerPhoneNumber { get; set; }
        public DateTime? StartLiquidationDate { get; set; }
        public DateTime? LiquidationPlanDate { get; set; }
        public DateTime? LiquidationPlanApprovalDate { get; set; }
        public DateTime? AuctionDate { get; set; }
        public string InsolvencyLaywerName { get; set; }
        public string InsolvencySolicitorName { get; set; }
        public string InsolvencyLaywerMail { get; set; }
        public string InsolvencySolicitorMail { get; set; }
        public string InsolvencyLaywerPhoneNumber { get; set; }
        public string InsolvencySolicitorPhoneNumber { get; set; }

        public virtual Participant ParticipantNavigation { get; set; }
        public virtual Portfolio PortfolioNavigation { get; set; }
    }
}
