<div align="center">
<img src="Assets/Icon.svg" width="192">

# CemuLauncher

![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/nxtroox/CemuLauncher/total?logo=github&logoColor=ffd429&label=Downloads&labelColor=191919&color=ffd429)
![GitHub License](https://img.shields.io/github/license/nxtroox/CemuLauncher?logo=github&logoColor=ffd429&label=License&labelColor=191919&color=ffd429)
![GitHub Release](https://img.shields.io/github/v/release/nxtroox/CemuLauncher?logo=github&logoColor=ffd429&label=Release&labelColor=191919&color=ffd429)

Automatically installs and updates nightly builds of the Cemu emulator.

</div>

## :cd: Installation

Install the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

Download the latest version from the [releases page](https://github.com/nxtroox/CemuLauncher/releases) and install it.

Alternatively, you can download a portable from the same page if you'd like.

<details>
<summary>Click here for instructions on how to compile it by yourself.</summary>

### :computer: Compile from source

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

2. Clone this repository and navigate into it:

    ```bash
    git clone https://github.com/nxtroox/CemuLauncher.git
    cd CemuLauncher
    ```

3. Run the following command to start compiling:

    ```bash
    dotnet publish -c Release -r win-x64
    ```

</details>

## :bulb: Usage

Instead of running Cemu, run CemuLauncher. It will automatically check for updates and install them.

It will create a portable installation of Cemu, so make sure to copy your data to it.

### :hammer_and_wrench: Configuration

After installing CemuLauncher, you can configure it using the configuration file (`config.yml`), which is located either under `%AppData%\CemuLauncher\config.yml` or next to the executable file.

Placing the configuration file next to the executable file turns on CemuLauncher's portable mode.

If not found, the configuration file will be created automatically inside your AppData folder (`%AppData%\CemuLauncher\config.yml`) when you launch CemuLauncher for the first time.

The configuration file should be self-explanatory.

## :scroll: License

This project is licensed under the [MIT License](LICENSE).

This project is not affiliated with Cemu or any other project.
