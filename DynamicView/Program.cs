using DynamicView.Controllers;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DbService>();

var app = builder.Build();

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
app.UseAuthorization();

// Routing Default to DynamicViewPage
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DynamicData}/{action=Index}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=DynamicReportList}/{action=Index}");



//app.MapControllerRoute(
//    name: "dynamicData",
//    pattern: "DynamicData/{reportId}",
//    defaults: new { controller = "DynamicData", action = "Index" }
//);

app.Run();
//app.Run("http://0.0.0.0:5000");  // Listen on all network interfaces (change port if needed)
//app.Run("http://192.168.29.3:5000");  // Listen on all network interfaces (change port if needed)
// local ip address 192.168.29.3   http://192.168.29.3:5000
