using DotNetGrill.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString)); // DI: Register a db connection to SQL Server
// after this line of code is executed, an instance of ApplicationDBContext will be available my classes
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// chaining methods for configuring the authentication/authorization mechanism
builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // needed for enabling [Authorize("Role")] decorator
    .AddEntityFrameworkStores<ApplicationDbContext>();

// chaining methods for configuring third-party authentication
builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("Authentication:Google")["ClientId"];
        options.ClientSecret = builder.Configuration.GetSection("Authentication:Google")["ClientSecret"];
    });

builder.Services.AddControllersWithViews();

builder.Services.AddSession(); // enables session service

builder.Services.AddSwaggerGen(); // enables swagger generator service

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); // enables the /swagger section in the website

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession(); // configures app to use session service
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
