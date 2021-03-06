using PMaP.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models
{
    public class ParticipantsModel
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public IEnumerable<Participant> Participants { get; set; }
    }
}
