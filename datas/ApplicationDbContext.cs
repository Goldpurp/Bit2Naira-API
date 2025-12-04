using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit2Naira_API.models;
using Microsoft.EntityFrameworkCore;

namespace Bit2Naira_API.datas
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}