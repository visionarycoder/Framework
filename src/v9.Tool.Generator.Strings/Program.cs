using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using v9.Ifx.Services.OS.Windows.Cli.Menu;
using v9.Ifx.Services.OS.Windows.Forms.Clipboard;
using v9.Tool.Generator.Strings;
using vc.Ifx.Services.Clipboard;
using vc.Ifx.Services.Generators;

// Start with host building to properly handle configuration
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((_, services) =>
    {

#if WINDOWS
        services.AddSingleton<IClipboardService, ClipboardService>();
#elif OSX
        services.AddSingleton<IClipboardService, vc.Ifx.Services.Mac.Clipboard.ClipboardService>();
#elif LINUX
        services.AddSingleton<IClipboardService, vc.Ifx.Services.Linux.Clipboard.ClipboardService>();
#endif

        services.AddSingleton<IStringGeneratorService, StringGeneratorService>();
        services.AddSingleton<IClipboardHelper, ClipboardHelper>();
        services.AddSingleton<IMenuService, MenuService>();

        services.AddSingleton<GeneratorClient>();

    })
    .Build();

// Retrieve the configured settings and services
var client = host.Services.GetRequiredService<GeneratorClient>();
await client.RunAsync(args).ConfigureAwait(false);