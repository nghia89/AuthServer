
using AuthorizationServer.Contracts;
using AuthorizationServer.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Persistence;

public class AuthServerContextSeed
{
    private readonly string AdminRoleName = "Admin";
    private readonly string UserRoleName = "Member";


    private readonly IPasswordHasher<AppUser> _passwordHasher = new PasswordHasher<AppUser>();

    public void SeedAsync(AuthServerContext context, ILogger<AuthServerContextSeed> logger)
    {
        if (!context.Users.Any())
        {
            var user = new AppUser()
            {
                Id = Guid.NewGuid(),
                FullName = "Admin",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                PhoneNumber = "0971660000",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, "Admin@123$");
            context.Users.Add(user);
            context.SaveChanges();
        }

        #region Quyền
        if (!context.Roles.Any())
        {
            var role = new AppRole
            {
                Name = AdminRoleName,
                Description = AdminRoleName,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
            };
            context.Roles.Add(role);
            context.SaveChanges();
        }

        #endregion Quyền
        context.SaveChanges();
    }
}