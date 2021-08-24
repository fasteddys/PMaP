using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class HomeModel
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public IEnumerable<Home> Documents { get; set; }
    }

    public class Home
    {
        public int idHome { get; set; }
        public string portfolio { get; set; }
        public string subportfolio { get; set; }
        public decimal nominalValue { get; set; }
        public decimal secured { get; set; }
        public decimal unsecured { get; set; }
        public string debtType { get; set; }
        public decimal priceSecured { get; set; }
        public decimal unsecuredPrice { get; set; }
        public string assignmentEntity { get; set; }
        public int acquisitionYear { get; set; }
    }
}
