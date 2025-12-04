using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Currency { get; set; }
        public double Balance { get; set; }
    }
}