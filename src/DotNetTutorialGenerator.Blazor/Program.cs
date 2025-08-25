using Blazored.LocalStorage;
using Blazored.Toast;
using DotNetTutorialGenerator.Blazor;
using DotNetTutorialGenerator.Blazor.Components;
using DotNetTutorialGenerator.Blazor.Components.Layout;
using DotNetTutorialGenerator.Blazor.Hubs;
using DotNetTutorialGenerator.Blazor.Services;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Services;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.GitHub;
using DotNetTutorialGenerator.Infrastructure.LLM;
using DotNetTutorialGenerator.Infrastructure.Persistence;
using DotNetTutorialGenerator.Infrastructure.Roslyn;
using DotNetTutorialGenerator.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(AppConstants.ExtraMimeTypes);
});

builder.Services.AddSignalR();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add data protection services with file system key protection for containerized environments
builder.Services.AddDataProtection()
    .SetApplicationName("DotNetTutorialGenerator")
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"));

// Add Blazored services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

// Add memory cache
builder.Services.AddMemoryCache();

// Add tutorial generator services
builder.Services.AddScoped<IRepositoryCrawler, LocalRepositoryCrawler>();
builder.Services.AddScoped<IRoslynAnalyzer, RoslynAnalyzer>();
builder.Services.AddScoped<ITutorialGenerator, TutorialFileWriter>();
builder.Services.AddScoped<ITutorialOrchestrator, TutorialOrchestrator>();

// Configure HttpClient with base URL
builder.Services.AddHttpClient("Api", client =>
{
    var apiUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:8081";
    client.BaseAddress = new Uri(apiUrl);
});

// Also register named HttpClient for direct injection
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<GitHubApiClient>();

// Add LLM service with configuration
builder.Services.AddScoped<ILLMService>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var llmSettings = config.GetSection("LLMSettings");
    var provider = llmSettings.GetValue<string>("Provider") ?? "openai";
    var apiKey = llmSettings.GetValue<string>("ApiKey") ?? "";
    var baseUrl = llmSettings.GetValue<string>("BaseUrl") ?? "http://localhost:1234/v1";

    return provider.ToLowerInvariant() switch
    {
        "openai" => new OpenAIService(apiKey),
        "claude" => new ClaudeService(apiKey),
        "lmstudio" => new LMStudioService(baseUrl),
        _ => new OpenAIService(apiKey)
    };
});

// Add missing core services
builder.Services.AddScoped<ITutorialOrchestrator, TutorialOrchestrator>();

// Add custom Blazor services
builder.Services.AddScoped<IAppState, AppState>();
builder.Services.AddScoped<ITutorialGenerationService, TutorialGenerationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCompression();
app.UseCors("AllowAll");
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add SignalR hub endpoints
app.MapHub<TutorialProgressHub>("/tutorialprogress");

app.Run();
