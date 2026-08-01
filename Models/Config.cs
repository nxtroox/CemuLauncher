namespace CemuLauncher.Models;

public class Config {
    public bool UpdatePrompt { get; set; } = false;
    public bool PassArguments { get; set; } = true;
    public string CemuPath { get; set; } = "cemu";
    public bool PortableCemu { get; set; } = true;
    public string DownloadPath { get; set; } = "downloads";
}
