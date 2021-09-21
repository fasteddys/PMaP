
namespace PMaP.Models
{
    public class Summary
    {
        public int Contracts { get; set; }
        public decimal TotalOB { get; set; }
        public decimal SecuredOB { get; set; }
        public decimal UnsecuredOB { get; set; }
        public decimal SecuredPrice { get; set; }
        public decimal UnsecuredPrice { get; set; }
        public int Debtors { get; set; }
        public int Guarantors { get; set; }
    }
}
