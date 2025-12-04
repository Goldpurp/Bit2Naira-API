using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Currency { get; set; }   // BTC, NGN, USDT
        public double Balance { get; set; }
        public string Type { get; set; }    // Buy or Sell
        public DateTime CreatedAt { get; set; }
    }
}