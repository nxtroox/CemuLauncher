using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using CemuLauncher.Services;
using Microsoft.Extensions.Localization;

namespace CemuLauncher.Models;

public sealed class Cemu(ConfigService configService, DownloadService downloadService, IStringLocalizer<Cemu> localizer, CemuService cemuService, PathService pathService) {
    public string? InstalledVersion { get; private set; }
    private string? NewVersion { get; set; }

    private const string DownloadUrl =
        "https://nightly.link/cemu-project/Cemu/workflows/build_check/main/cemu-bin-windows-x64.zip";

    private readonly Config _config = configService.Config;

    public void Launch() {
        if (!File.Exists(pathService.ExecutablePath))
            throw new FileNotFoundException(localizer["CemuNotFoundError"], pathService.ExecutablePath);

        var startInfo = new ProcessStartInfo() {
            FileName = pathService.ExecutablePath,
            WorkingDirectory = pathService.CemuPath,
            UseShellExecute = true
        };

        if (_config.PassArguments) {
            foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
                startInfo.ArgumentList.Add(arg);
        }

        Process.Start(startInfo);
    }

    public async Task<bool> NeedsUpdateAsync(CancellationToken cancellationToken = default) {
        InstalledVersion ??= await cemuService.GetLocalVersionAsync(cancellationToken);
        NewVersion ??= await cemuService.GetLatestVersionAsync(cancellationToken);

        return InstalledVersion != NewVersion;
    }

    public async Task InstallAsync(IProgress<double>? downloadProgress = null, CancellationToken cancellationToken = default) {
        if (NewVersion is null)
            throw new InvalidOperationException(localizer["NewVersionNotInitializedError"]);

        Directory.CreateDirectory(pathService.BasePath);
        Directory.CreateDirectory(pathService.CemuPath);
        Directory.CreateDirectory(pathService.DownloadPath);

        try {
            await downloadService.DownloadAsync(
                DownloadUrl, pathService.DownloadPath, PathService.ZipFileName, downloadProgress, cancellationToken);

            await ZipFile.ExtractToDirectoryAsync(
                pathService.ZipFilePath, pathService.CemuPath, overwriteFiles: true, cancellationToken);

            ApplyPortable();

            InstalledVersion = NewVersion;

            await File.WriteAllTextAsync(pathService.VersionFilePath, InstalledVersion, cancellationToken);
        } finally {
            if (File.Exists(pathService.ZipFilePath))
                File.Delete(pathService.ZipFilePath);
        }
    }

    private void ApplyPortable() {
        var portablePath = Path.Combine(pathService.CemuPath, "portable");
        var disabledPath = Path.Combine(pathService.CemuPath, "portable.disabled");

        if (_config.PortableCemu) {
            if (Directory.Exists(disabledPath))
                Directory.Move(disabledPath, portablePath);
            else
                Directory.CreateDirectory(portablePath);
        } else if (Directory.Exists(portablePath))
            Directory.Move(portablePath, disabledPath);
    }
}
