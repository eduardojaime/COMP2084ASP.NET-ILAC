using DotNetGrillWebUI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// Builder object manages the app's configuration and services
var builder = WebApplication.CreateBuilder(args);

// Dependency Injection for the DbContext service
// Add services to the builder.Services collection.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Dependency Injection for the Identity service
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Dependency Injection for a third-party authentication service
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// Dependency injection for Razor Pages
builder.Services.AddControllersWithViews();
// Dependency injection for Session services
builder.Services.AddSession();
// You must register all services before the following line of code

// Add Swagger to services collection
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the app to use Swagger
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNetGrillWebUI v1"));

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

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Registering services is a two-step process
// First add the service to the builder.Services collection
// Then call Use* to make your app utilize the service
// UseAuthentication must be called before UseAuthorization to enable third-party login services
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
// This line of code executes the app
app.Run();
