using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bit2Naira_API.dtos
{
    public record UsersDto(
        string fullName,
        string email,
        string phoneNumber,
        string password,
        string avatarUrl,
        string location,
        string occupation
    );
}
