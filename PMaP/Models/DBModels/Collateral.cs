using System;

namespace PMaP.Models.DBModels
{
    public class Collateral
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public string Portfolio { get; set; }
        public string Subportfolio { get; set; }
        public DateTime? ProcessDate { get; set; }
        public int? ContractId { get; set; }
        public string Contract { get; set; }
        public string CollateralId { get; set; }
        public string CollateralType { get; set; }
        public string CollateralSubtype { get; set; }
        public string CollateralUsage { get; set; }
        public string Status { get; set; }
        public string ConstuctionYear { get; set; }
        public int? SurfaceM2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public DateTime? InitialAppraisalDate { get; set; }
        public decimal? InitialAppraisalValue { get; set; }
        public DateTime? LastAppraisalDate { get; set; }
        public decimal? LastAppraisalValue { get; set; }
        public int? RegistryAssetId { get; set; }
        public string LandRegistryTown { get; set; }
        public string LandRegistryNumber { get; set; }
        public string CadastralReference { get; set; }
        public string Idufir { get; set; }
        public decimal? Liens { get; set; }

        public virtual Contract ContractNavigation { get; set; }
        public virtual Portfolio PortfolioNavigation { get; set; }
    }
}
