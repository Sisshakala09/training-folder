using DocumentFormat.OpenXml.Bibliography;
using HRISApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepoContextNamespace;
using Repos.Repos;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));




var conn = builder.Configuration.GetConnectionString("RepoContext") ?? throw new InvalidOperationException("Connection string 'RepoContext' not found.");
builder.Services.AddDbContext<RepoContext>(options =>
    options.UseSqlServer(conn));

builder.Services.AddScoped<IDept, Dept>();
builder.Services.AddScoped<DepartmentService, DepartmentService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IRepogeneric<Department>, Repogeneric<Department>>();

builder.Services.AddScoped<ServiceClass<Department>,ServiceClass<Department>>();
//builder.Services.AddScoped<IRepogeneric<Employee>, Repogeneric<Employee>>();


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
