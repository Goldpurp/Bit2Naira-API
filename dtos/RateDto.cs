using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.dtos
{
    public record RateDto(
       [Required] double nairaRate,
       [Required] string fromBitcoin,
       [Required] string toNaira);
}