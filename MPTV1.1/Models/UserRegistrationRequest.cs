using System;
namespace College_API_V1.Models
{
    public class UserRegistrationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }

        public string EmployeeRole { get; set; }
    }
}
