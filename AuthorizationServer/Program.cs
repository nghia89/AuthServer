using AuthorizationServer.Extensions;
using AuthorizationServer.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
                .UseDbContext<AuthServerContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("connect/authorize")
                .SetLogoutEndpointUris("connect/logout")
                .SetTokenEndpointUris("connect/token");

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        options.AllowAuthorizationCodeFlow();

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableTokenEndpointPassthrough();
    });
builder.Services.AddTransient<AuthorizationService>();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/account/login";
        });
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(c =>
//     {
//         c.LoginPath = "/Authenticate";
//     });
builder.Services.AddTransient<ClientsSeeder>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7002")
            .AllowAnyHeader();
    });
});
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ClientsSeeder>();
    seeder.AddClients().GetAwaiter().GetResult();
    seeder.AddScopes().GetAwaiter().GetResult();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MigrateDatabase<AuthServerContext>((context, services) =>
    {
        var logger = services.GetService<ILogger<AuthServerContextSeed>>();
        new AuthServerContextSeed().SeedAsync(context, logger);
    }).Run();