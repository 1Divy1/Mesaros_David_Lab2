using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mesaros_David_Lab2.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// politica de autorizare: acces doar pentru utilizatorii cu rol de Admin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    // restrictioneaza accesul pe folderul /Books
    options.Conventions.AuthorizeFolder("/Books");
    // permite accesul anonim la paginile Index si Details din folderul /Books
    options.Conventions.AllowAnonymousToPage("/Books/Index");
    options.Conventions.AllowAnonymousToPage("/Books/Details");
    // sectiunile Members, Publishers si Categories sunt accesibile doar pentru Admin
    options.Conventions.AuthorizeFolder("/Members", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Publishers", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Categories", "AdminPolicy");
});

builder.Services.AddDbContext<Mesaros_David_Lab2Context>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Mesaros_David_Lab2Context") ?? throw new InvalidOperationException("Connection string 'Mesaros_David_Lab2Context' not found.")));

builder.Services.AddDbContext<LibraryIdentityContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("LibraryIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'LibraryIdentityContextConnection' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
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