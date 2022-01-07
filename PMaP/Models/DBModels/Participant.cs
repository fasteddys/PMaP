using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMaP.Models.DBModels
{
    public partial class Participant
    {
        public Participant()
        {
            Insolvencies = new HashSet<Insolvency>();
        }

        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public string Portfolio { get; set; }
        public string Subportfolio { get; set; }
        public DateTime? ProcessDate { get; set; }
        public int? ContractId { get; set; }
        public string Contract { get; set; }
        public string ParticipantId { get; set; }
        public string Name { get; set; }
        public string Identification { get; set; }
        public string PersonType { get; set; }
        public string DebtorType { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Cno { get; set; }
        public string Cnae { get; set; }
        public string CnaeDescription { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Nationality { get; set; }
        public string Resident { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Email { get; set; }

        public virtual Contract ContractNavigation { get; set; }
        public virtual Portfolio PortfolioNavigation { get; set; }
        public virtual ICollection<Insolvency> Insolvencies { get; set; }
    }
}
