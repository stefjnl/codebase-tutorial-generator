using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleConsoleApp.Services;

// Create host builder
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register services
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IFileService, FileService>();
    })
    .Build();

// Get service and run
var messageService = host.Services.GetRequiredService<IMessageService>();
messageService.DisplayWelcomeMessage();

var fileService = host.Services.GetRequiredService<IFileService>();
fileService.ProcessFile("sample.txt");

await host.RunAsync();
