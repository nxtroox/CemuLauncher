using System.IO;
using CemuLauncher.Models;

namespace CemuLauncher.Services;

public sealed class PathService(ConfigService configService) {
    private readonly Config _config = configService.Config;

    private string ParseRootedPath(string path) =>
        Path.IsPathRooted(path) ? path : Path.Combine(BasePath, path);

    public string BasePath {
        get {
            if (field is not null)
                return field;

            if (Path.Exists(Path.Combine(AppContext.BaseDirectory, "config.yml")))
                field = AppContext.BaseDirectory;
            else
                field = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CemuLauncher");

            return field;
        }
    }

    public string CemuPath =>
        field ??= ParseRootedPath(_config.CemuPath);

    public string DownloadPath =>
        field ??= ParseRootedPath(_config.DownloadPath);

    public string ExecutablePath =>
        Path.Combine(CemuPath, "Cemu.exe");

    public string VersionFilePath =>
        Path.Combine(BasePath, "version.txt");

    public const string ZipFileName = "cemu-bin-windows-x64.zip";
    public string ZipFilePath =>
        Path.Combine(DownloadPath, ZipFileName);
}
