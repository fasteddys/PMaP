using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models.DBModels
{
    public class PortfolioContract
    {
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public int? ContractId { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Portfolio Portfolio { get; set; }
    }
}
