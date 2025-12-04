using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.dtos
{
    public record UpdateProfileDto(
        string? fullName,
        string? phoneNumber,
        IFormFile? profilePicture,
        string? location,
        string? occupation
    );

}