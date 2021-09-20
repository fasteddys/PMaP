using PMaP.Models.DBModels;
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
        public List<Contract> Contracts { get; set; }
        public Summary Summary { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Investor> Investors { get; set; }
        public List<Procedure> Procedures { get; set; }
    }
}
