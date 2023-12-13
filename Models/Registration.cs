using System;


namespace eCom.Models
{
    public class Registration
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int IsActive { get; set; }

    }
}
