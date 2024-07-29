using ContaFacil.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ContaFacil.Utilities;
using ContaFacil.Controllers;
using Quartz;
var builder = WebApplication.CreateBuilder(args);

// Añadir servicios al contenedor...
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
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("TareaEnviarFacturacionSRI");
    q.AddJob<TareaEnviarFacturacionSRI>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("TareaEnviarFacturacionSRI-trigger")
        .WithCronSchedule("0 0/5 * * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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
});

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    // Usa el manejador de excepciones global
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

// Manejador de errores global
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.Run();