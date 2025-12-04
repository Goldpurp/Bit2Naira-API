using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit2Naira_API.datas;

namespace Bit2Naira_API.models
{
    public class LogTransaction
    {

        private readonly ApplicationDbContext _db;

        private void LogUserTransaction(int userId, string currency, double balance, string type)
        {
            var tx = new Transaction
            {
                UserId = userId,
                Currency = currency,
                Balance = balance,
                Type = type,
            };

            _db.Transactions.Add(tx);
            _db.SaveChanges();
        }
    }
}