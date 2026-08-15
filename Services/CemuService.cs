using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace CemuLauncher.Services;

public sealed class CemuService(HttpClient httpClient, PathService pathService) {
    private const string LatestCommitUrl = "https://api.github.com/repos/cemu-project/Cemu/commits/main";

    public async Task<string?> GetLatestVersionAsync(CancellationToken cancellationToken = default) {
        try {
            using var response = await httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, LatestCommitUrl), cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (doc.RootElement.TryGetProperty("sha", out var shaElement))
                return shaElement.GetString();

            return null;
        } catch {
            return null;
        }
    }

    public async Task<string?> GetLocalVersionAsync(CancellationToken cancellationToken = default) {
        if (!File.Exists(pathService.VersionFilePath))
            return null;

        try {
            return await File.ReadAllTextAsync(pathService.VersionFilePath, cancellationToken);
        } catch (FileNotFoundException) {
            return null;
        } catch (DirectoryNotFoundException) {
            return null;
        }
    }
}
