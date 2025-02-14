using Asp.Versioning;
using DoodieViewer.Server;
using DoodieViewer.Server.Middleware;
using DoodieViewer.Server.Service;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddLocalization();
    builder.Services.AddControllers();
    builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = ApiVersionReader.Combine(ApiVersionReader.Combine(new HeaderApiVersionReader("X-Api-Version"), new QueryStringApiVersionReader("api-version")), new UrlSegmentApiVersionReader());
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "DoodieViewer.Server", Version = "v1" });
    });

    builder.Services.AddSingleton<UrlBindService>();
    builder.Services.AddSingleton<PageParserService>();

    builder.Services.AddExceptionHandler<ServerExceptionHandler>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "DoodieViewer.Server v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // map route when access root to /swagger
    app.MapGet("/", () =>
    {
        return Results.Redirect("/swagger");
    });

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped because of exception");
}
finally
{
    LogManager.Shutdown();
}