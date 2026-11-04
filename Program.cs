using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mesaros_David_Lab2.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<Mesaros_David_Lab2Context>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Mesaros_David_Lab2Context") ?? throw new InvalidOperationException("Connection string 'Mesaros_David_Lab2Context' not found.")));

builder.Services.AddDbContext<LibraryIdentityContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("LibraryIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'LibraryIdentityContextConnection' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<LibraryIdentityContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();