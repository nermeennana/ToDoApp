using System;
using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Models;

public class AppUser : IdentityUser
{
    // IdentityUser already has properties like Id, UserName, Email, PasswordHash, etc.
    public string DisplayName { get; set; } = null!;
}
