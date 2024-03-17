using AuthorizationServer.Contracts;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Entities;


public class AppRole : IdentityRole<Guid>, IAuditable
{
    public string Description { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}