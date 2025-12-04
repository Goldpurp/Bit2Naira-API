using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit2Naira_API.dtos;

// using Bit2Naira_API.dtos;
using Bit2Naira_API.models;
using Microsoft.AspNetCore.Mvc;

namespace Bit2Naira_API.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController : ControllerBase
    {
        public static Rate rate = new(0, "BTC", "NGN");

        [HttpGet("get-naira-rate")]
        public IActionResult GetRate()
        {
            var currentRate = rate.GetRate();
            return Ok(currentRate);
        }

        [HttpPost("set-naira-rate")]
        public IActionResult SetRate([FromBody] RateDto rateDto)
        {
            if (rateDto.fromBitcoin.ToLower() != "btc")
            {
                return BadRequest("bad request, we only support BTC to NGN exchange rate");
            }
            if (rateDto.toNaira.ToLower() != "ngn")
            {
                return BadRequest("bad request, we only support BTC to NGN exchange rate");
            }

            rate.UpdateNairaRate(rateDto.nairaRate);
            return Ok(new
            {
                Message = "Rate changed sucessfully"
            });
        }

    }
}