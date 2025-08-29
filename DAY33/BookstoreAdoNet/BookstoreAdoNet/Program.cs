using BookstoreAdoNet.Data;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
// Register the repository
builder.Services.AddScoped<BookRepository>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();


app.UseAuthorization();


app.MapControllerRoute(
name: "default",
pattern: "{controller=Books}/{action=Index}/{id?}");


app.Run();