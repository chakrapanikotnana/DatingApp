using System;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
 string GenerateToken(AppUser appUser);
}