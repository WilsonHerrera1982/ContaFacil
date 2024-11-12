using ContaFacil.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ContaFacil.Utilities;
using ContaFacil.Controllers;
using Quartz;
using OfficeOpenXml;
using ContaFacil.Models.Interfaces;
using ContaFacil.Models.Services;
using ContaFacil.Services.Impl;
using ContaFacil.Services;

var builder = WebApplication.CreateBuilder(args);

// Establecer el contexto de licencia de EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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
    options.UseMySql(
                "Server=localhost;Database=contable;Uid=root;Pwd=ROOT;AllowZeroDateTime=true;ConvertZeroDateTime=true;",
                new MySqlServerVersion(new Version(8, 0, 30)), // Ajusta la versión según tu servidor MySQL
                options =>
                {
                    options.MigrationsAssembly("MyProject");
                }); 
});

builder.Services.AddTransient<FacturaController>();
builder.Services.AddTransient<FacturaXmlGenerator>();
builder.Services.AddScoped<IReporteMayorizacionService, ReporteMayorizacionService>();
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
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("TareaRegistroTransacciones");
    q.AddJob<TareaRegistroTransacciones>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("TareaRegistroTransacciones-trigger")
        .WithCronSchedule("0 */2 * * * ?")); // Cada 5 minutos
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 64; // Aumenta la profundidad máxima si es necesario
});
builder.Services.AddScoped<TareaRegistroTransacciones>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOpcionCliente, OpcionClienteImpl>();
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
app.UseSessionTimeout();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

// Manejador de errores global
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.Run();