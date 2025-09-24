using Amazon.S3;
using AspNetCoreRateLimit;
using CRM_Homestay.App.Attributes;
using CRM_Homestay.App.Extensions;
using CRM_Homestay.App.Middlewares;
using CRM_Homestay.App.ServiceInstallers;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.InstallServices(builder.Configuration);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddSignalR();

//Global filter
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = CustomFluentResponse.FluentValidationResponse;
});

// Add LogoutCronJobService
builder.Services.AddCommandHandlers();

var app = builder.Build();
app.ApplyMigrations();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "HOMESTAY API");
        c.RoutePrefix = string.Empty;
    });
}
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}
);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/static-files"
});
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("ApiCorsPolicy");
app.UseMiddleware<AccessControlMiddleware>();
app.UseMiddleware<TokenAuthorizationMiddleware>();
app.UseMiddleware<LocalizationMiddleware>();
app.MapControllers();
app.UseResponseCompression();

app.Run();