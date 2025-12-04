using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Location { get; set; }
        public string? Occupation { get; set; }

        public ICollection<Wallet> Wallets { get; set; }
    }
}