using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using MongoDB.Driver;
using MVCExample.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


var connectionString1 = builder.Configuration.GetConnectionString("OrgContext") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString1));


//builder.Services.Configure<MongoDBSettings>(
//    builder.Configuration.GetSection("MongoDBSettings"));

//builder.Services.AddSingleton<IMongoClient>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
//    return new MongoClient(settings.ConnectionString);
//});

//builder.Services.AddScoped(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
//    var client = sp.GetRequiredService<IMongoClient>();
//    return client.GetDatabase(settings.DatabaseName);
//});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
