using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit2Naira_API.dtos;

namespace Bit2Naira_API.models
{
    public class Rate
    {
        public double NairaRate { get; private set; }
        public string FromBitcoin { get; private set; }
        public string ToNaira { get; private set; }


        public Rate(double nairaRate, string fromBitcoin, string toNaira)
        {
            NairaRate = nairaRate;
            FromBitcoin = fromBitcoin;
            ToNaira = toNaira;
        }


        public void UpdateNairaRate(double newNairaRate)
        {
            NairaRate = newNairaRate;
        }

        public RateDto GetRate()
        {
            return new RateDto(nairaRate: NairaRate, fromBitcoin: FromBitcoin, toNaira: ToNaira);
        }


    }
}