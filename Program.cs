using ContaFacil.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ContaFacil.Utilities;
using ContaFacil.Controllers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration.GetConnectionString("ContableContext");
Console.WriteLine($"ConnectionString: {connectionString}");

builder.Services.AddDbContext<ContableContext>(options =>
{
    options.UseNpgsql("Host=localhost;Database=contable;Username=postgres;Password=postgres");
});
builder.Services.AddTransient<FacturaController>();
builder.Services.AddTransient<FacturaXmlGenerator>();
var app = builder.Build();
var supportedCultures = new[]
{
    new CultureInfo("es-US")
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
