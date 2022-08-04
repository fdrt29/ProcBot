using System.ComponentModel.DataAnnotations;

namespace ProcBot;

public class Models
{
    public class User
    {
        [Key] public long UserId { get; set; }
        public int? CompanyId { get; set; }

        public State State { get; set; }
        // public string Name;
        // public string Email;
        // public string Password;
    }

    public class Bill
    {
        [Key] public int BillId { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string Email { get; set; }
    }

    public class Company // TODO delete?
    {
        [Key] public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}