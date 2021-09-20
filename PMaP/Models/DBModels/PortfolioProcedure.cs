using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models.DBModels
{
    public partial class PortfolioProcedure
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public int? ProcedureId { get; set; }

        public virtual Portfolio Portfolio { get; set; }
        public virtual Procedure Procedure { get; set; }
    }
}
