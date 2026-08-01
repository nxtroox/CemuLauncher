using System.IO;
using System.Text;
using CemuLauncher.Models;
using YamlDotNet.Serialization;

namespace CemuLauncher.Services;

public sealed class ConfigService(IDeserializer deserializer, ISerializer serializer) {
    private Config? _cached;
    private string? _path;

    public async Task<Config> GetAsync() =>
        _cached ??= await LoadAsync(_path ??= GetPath());

    public Config Config =>
        _cached ?? throw new InvalidOperationException("Config not loaded");

    private static string GetPath() {
        var appDataConfig = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "CemuLauncher", "config.yml");

        var exeConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "config.yml");

        return File.Exists(exeConfig) ? exeConfig : appDataConfig;
    }

    private async Task<Config> LoadAsync(string path) {
        if (!File.Exists(path)) {
            var config = new Config();
            var yaml = serializer.Serialize(config);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllBytesAsync(path, Encoding.UTF8.GetBytes(yaml));

            return config;
        }

        try {
            var content = await File.ReadAllTextAsync(path);
            return deserializer.Deserialize<Config>(content);
        } catch {
            return new Config();
        }
    }
}
