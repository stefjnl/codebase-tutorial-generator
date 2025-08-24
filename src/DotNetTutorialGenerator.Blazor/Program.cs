using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DotNetTutorialGenerator.Blazor;
using DotNetTutorialGenerator.Blazor.Components;
using DotNetTutorialGenerator.Blazor.Components.Layout;
using DotNetTutorialGenerator.Blazor.Services;
using DotNetTutorialGenerator.Blazor.Hubs;
using Blazored.LocalStorage;
using Blazored.Toast;

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

// Add Blazored services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

// Add custom services
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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add SignalR hub endpoints
app.MapHub<TutorialProgressHub>("/tutorialprogresshub");

app.Run();
