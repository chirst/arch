using Arch.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();

builder.Services.AddControllers();
builder.Services.AddScoped<MigrationRepository>();
builder.Services.AddScoped<UserRepository>();

builder
    .Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddUserStore<UserRepository>()
    .AddRoleStore<RoleRepository>()
    .AddDefaultTokenProviders();

var app = builder.Build();
app.Migrate();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();
