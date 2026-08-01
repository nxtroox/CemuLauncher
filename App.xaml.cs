using System.Net.Http.Headers;
using System.Reflection;
using System.Windows;
using CemuLauncher.Models;
using CemuLauncher.Services;
using CemuLauncher.ViewModels;
using CemuLauncher.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CemuLauncher;

public partial class App : Application {
    public static IHost? AppHost { get; private set; }

    public App() {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                services.AddHttpClient("Default", client => {
                    client.DefaultRequestHeaders.UserAgent.Add(
                        new ProductInfoHeaderValue(
                            "CemuLauncher",
                            Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                        )
                    );
                });

                services.AddSingleton<DownloadService>();

                services.AddSingleton(_ => new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build());

                services.AddSingleton(_ => new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build());

                services.AddSingleton<ConfigService>();

                services.AddSingleton<Cemu>();

                services.AddSingleton<MainWindow>();
                services.AddTransient<MainViewModel>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e) {
        await AppHost!.StartAsync();

        await AppHost.Services
            .GetRequiredService<ConfigService>()
            .GetAsync();

        var window = AppHost.Services.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e) {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
