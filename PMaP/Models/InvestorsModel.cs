using PMaP.Models.DBModels;
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
}
