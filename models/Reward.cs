using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.models
{
    public class Reward
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Points { get; set; }
    }
}