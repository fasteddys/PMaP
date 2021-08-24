using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class ContractsModel
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public List<Contract> Documents { get; set; }
        public Summary Summary { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
    }

    public class Contract
    {
        public int Id { get; set; }
        public string ContractId { get; set; }
        public int? PortfolioId { get; set; }
        public string Portfolio { get; set; }
        public string Subportfolio { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string ContractType { get; set; }
        public string PerformingStatus { get; set; }
        public string DebtType { get; set; }
        public string MainParticipantId { get; set; }
        public int? NumParticipants { get; set; }
        public int? NumGuarantors { get; set; }
        public string OriginalEntity { get; set; }
        public decimal? TotalAmountOb { get; set; }
        public string Currency { get; set; }
        public decimal? MaturityPrincipalBalance { get; set; }
        public decimal? OutstandingPrincipalBalance { get; set; }
        public decimal? OrdinaryInterests { get; set; }
        public decimal? DefaultInterests { get; set; }
        public DateTime? OriginationDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public sbyte? EarlyMaturity { get; set; }
        public DateTime? EarlyMaturityDate { get; set; }
        public DateTime? FirstUnpaidInstalmentDate { get; set; }
        public DateTime? DefaultDate { get; set; }
        public int? UnpaidInstalments { get; set; }
        public int? OutstandingInstalments { get; set; }
        public string AccountingSituation { get; set; }
        public int? LtvOriginal { get; set; }
        public sbyte? Syndicated { get; set; }
        public decimal? SyndicationPercent { get; set; }
        public sbyte? Securitized { get; set; }
        public string SecurityNumber { get; set; }
        public sbyte? Novation { get; set; }
        public DateTime? NovationDate { get; set; }
        public sbyte? JudicialProcess { get; set; }
        public string ProceedingType { get; set; }
        public sbyte? LegalProcess { get; set; }
        public string Insolvency { get; set; }
        public sbyte? Reo { get; set; }
        public decimal? ReoAmount { get; set; }

        public virtual ICollection<Investor> Investors { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<PortfolioContract> PortfolioContracts { get; set; }
    }

    public class Assessment
    {
        public int contracts { get; set; }
        public decimal totalOB { get; set; }
        public decimal securedOB { get; set; }
        public decimal unsecuredOB { get; set; }
        public decimal securedPrice { get; set; }
        public decimal unsecuredPrice { get; set; }
        public int debtors { get; set; }
        public int guarantors { get; set; }
    }

    public class PortfolioContract
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public int? ContractId { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Portfolio Portfolio { get; set; }
    }
}
