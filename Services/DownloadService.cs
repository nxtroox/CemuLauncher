using System.IO;
using System.Net.Http;

namespace CemuLauncher.Services;

public sealed class DownloadService(HttpClient httpClient) {
    public async Task DownloadAsync(string url, string downloadPath, string fileName, IProgress<double>? progress, CancellationToken cancellationToken = default) {
        var fullPath = Path.Combine(downloadPath, fileName);

        Directory.CreateDirectory(downloadPath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength;

        if (contentLength == null) {
            progress?.Report(-1);
        }

        await using var downloadStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: true);

        var buffer = new byte[81920];
        long totalRead = 0;
        int read;

        var lastReport = DateTime.UtcNow;

        while ((read = await downloadStream.ReadAsync(buffer, cancellationToken)) > 0) {
            await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            totalRead += read;

            if (contentLength.HasValue && contentLength.Value > 0 && DateTime.UtcNow - lastReport > TimeSpan.FromMilliseconds(250)) {
                var percent = (double)totalRead / contentLength.Value * 100.0;
                progress?.Report(Math.Clamp(percent, 0.0, 100.0));
                lastReport = DateTime.UtcNow;
            }
        }

        progress?.Report(100);
    }
}
