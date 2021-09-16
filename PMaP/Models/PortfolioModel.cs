﻿using PMaP.Models.ViewModels.Portfolio;
using PMaP.Models.ViewModels.PortfolioValuation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class PortfolioModel
    {
        public ViewModels.Portfolio.ViewModel ViewModel { get; set; }

        public int ResponseCode { get; set; }
        public string Message { get; set; }

        public Summary Summary { get; set; }
        public List<Portfolio> Documents { get; set; }
        public List<Contract> Contracts { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
        public List<Procedure> Procedures { get; set; }

        public string ActiveTab { get; set; }

        [Required]
        public string Portfolio { get; set; }
    }

    public class Portfolio
    {
        public int Id { get; set; }
        public string HolderEntity { get; set; }
        public string Portfolio1 { get; set; }
        public string Subportfolio { get; set; }
        public string OperationType { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? CutOffDate { get; set; }
        public DateTime? SigningDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public decimal? CutOffOb { get; set; }
        public decimal? SigningOb { get; set; }
        public decimal? ClosingOb { get; set; }
        public string Investor { get; set; }
        public string Tipology { get; set; }
        public int? Contracts { get; set; }
        public int? Year { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Contract> ContractsNavigation { get; set; }
        public virtual ICollection<Home> Homes { get; set; }
        public virtual ICollection<Investor> Investors { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<PortfolioContract> PortfolioContracts { get; set; }
        public virtual ICollection<PortfolioProcedure> PortfolioProcedures { get; set; }
        public virtual ICollection<Procedure> Procedures { get; set; }
    }
}
