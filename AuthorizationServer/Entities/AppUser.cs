


using AuthorizationServer.Contracts;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Entities;

public class AppUser : IdentityUser<Guid>, IAuditable
{
    public string FullName { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}