using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bit2Naira_API.datas;
using Bit2Naira_API.dtos.WalletDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bit2Naira_API.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly datas.ApplicationDbContext _db;
        public WalletController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpPost("deposit-naira")]
        public IActionResult DepositNaira([FromBody] DepositDto depositDto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user id missing");
            }

            var userId = int.Parse(userIdClaim.Value);
            var ngnWallet = _db.Wallets.FirstOrDefault(u => u.UserId == userId && u.Currency == "NGN");
            if (ngnWallet == null)
            {
                return NotFound("wallet not found");
            }
            if (depositDto.DepositAmount <= 0)
            {
                return BadRequest("deposit amount must be greater than zero");
            }

            ngnWallet.Balance += depositDto.DepositAmount;

            _db.SaveChanges();
            return Ok(new
            {
                Message = $"you have successfully deposit ₦{depositDto.DepositAmount}",
                ngnWallet
            });
        }

        [HttpPost("withdraw-naira")]
        public IActionResult WithdrawNaira([FromBody] WithdrawDto withdrawDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user id missing");
            }
            var userId = int.Parse(userIdClaim.Value);

            var ngnWallet = _db.Wallets.FirstOrDefault(n => n.UserId == userId && n.Currency == "NGN");
            if (ngnWallet == null)
            {
                return NotFound("wallet not found");
            }
            if (ngnWallet.Balance < withdrawDto.WithdrawAmount)
            {
                return BadRequest("insufficient balance");
            }

            ngnWallet.Balance -= withdrawDto.WithdrawAmount;

            _db.SaveChanges();
            return Ok(new
            {
                Message = $"you have successfully withdraw ₦{withdrawDto.WithdrawAmount}",
                ngnWallet
            });
        }

       [HttpPost("buy-bitcoin")]
        public IActionResult BuyBtc([FromBody] BuyDto buyDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user id missing");
            }
            var userId = int.Parse(userIdClaim.Value);

            var ngnWallet = _db.Wallets.FirstOrDefault(n => n.UserId == userId && n.Currency == "NGN");
            var btcWallet = _db.Wallets.FirstOrDefault(b => b.UserId == userId && b.Currency == "BTC");

            if (ngnWallet == null && btcWallet == null)
            {
                return NotFound("wallet not found");
            }

            if (ngnWallet.Balance < buyDto.NairaAmount)
            {
                return BadRequest(" Naira Balance is Insufficient");
            }

            var btcToNairaRateDto = RateController.rate.GetRate();
            var btcToNairaRate = btcToNairaRateDto.nairaRate;

            var btcBalance = buyDto.NairaAmount / btcToNairaRate;

            btcWallet.Balance += btcBalance;
            ngnWallet.Balance -= buyDto.NairaAmount;

            _db.SaveChanges();


            return Ok(new
            {
                Message = "purchased successfully",
                NairaBalance = ngnWallet.Balance,
                BitcoinBalance = btcWallet.Balance
            });
        }
 
        [HttpPost("sell-bitcoin")]
        public IActionResult SellBtc([FromBody] SellDto sellDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user id missing");
            }
            var userId = int.Parse(userIdClaim.Value);

            var ngnWallet = _db.Wallets.FirstOrDefault(n => n.UserId == userId && n.Currency == "NGN");
            var btcWallet = _db.Wallets.FirstOrDefault(b => b.UserId == userId && b.Currency == "BTC");

            if (ngnWallet == null && btcWallet == null)
            {
                return NotFound("wallet not found");
            }

            if (btcWallet.Balance < sellDto.BitcoinAmount)
            {
                return BadRequest("Bitcoin Balance is Insufficient");
            }

            var btcToNairaRateDto = RateController.rate.GetRate();
            var btcToNairaRate = btcToNairaRateDto.nairaRate;

            var ngnBalance = sellDto.BitcoinAmount * btcToNairaRate;

            btcWallet.Balance -= sellDto.BitcoinAmount;
            ngnWallet.Balance += ngnBalance;

            _db.SaveChanges();
            return Ok(new
            {
                Message = "purchased successfully",
                BitcoinBalance = btcWallet.Balance,
                NairaBalance = ngnWallet.Balance
            });
        }
    }
}