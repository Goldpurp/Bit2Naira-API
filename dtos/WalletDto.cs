using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.dtos
{
    public record WalletDto(
        int userId,
        decimal nairaBalance,
        decimal bitcoinBalance
    );
}