using System.Windows;
using CemuLauncher.Models;
using CemuLauncher.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Localization;

namespace CemuLauncher.ViewModels;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    public partial string? Status { get; set; }

    [ObservableProperty]
    public partial bool ProgressIsIndeterminate { get; set; } = true;

    [ObservableProperty]
    public partial double ProgressValue { get; set; } = -1;

    public IProgress<double> Progress { get; }

    private readonly Cemu _cemu;
    private readonly IStringLocalizer _localizer;
    private readonly Config? _config;

    public MainViewModel(Cemu cemu, IStringLocalizer<MainViewModel> localizer, ConfigService configService) {
        _cemu = cemu;
        _localizer = localizer;
        _config = configService.Config;

        Status = _localizer["UpdateCheck"];

        Progress = new Progress<double>(p => {
            if (p < 0) {
                ProgressIsIndeterminate = true;
            } else {
                Status = _localizer["UpdateAvailable"];
                ProgressIsIndeterminate = false;
                ProgressValue = p;
            }
        });
    }

    private bool PromptUpdate() =>
        !_config!.UpdatePrompt ||
        MessageBox.Show(
            _localizer["UpdatePrompt"],
            _localizer["UpdateAvailable"],
            MessageBoxButton.YesNo,
            MessageBoxImage.Information)
        == MessageBoxResult.Yes;

    public async Task OnWindowLoadedAsync(CancellationToken cancellationToken = default) {
        try {
            var needsUpdate = await _cemu.NeedsUpdateAsync(cancellationToken);
            var promptResult = PromptUpdate();

            if (needsUpdate && promptResult)
                await _cemu.InstallAsync(Progress, cancellationToken);

            _cemu.Launch();

            Application.Current.Shutdown();
        } catch (Exception ex) {
            Status = string.Join(" ", [_localizer["ErrorPrefix"], ex.Message]);
        }
    }
}
