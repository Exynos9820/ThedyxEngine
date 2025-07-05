# ThedyxEngine Documentation

Welcome to the documentation of ThedyxEngine, a 2D physics engine designed to simulate heat transfer across different materials using a visually intuitive approach.

## Overview

ThedyxEngine is a 2D physics engine designed to simulate heat transfer across different materials using a visually intuitive approach. 
The engine supports various forms of heat transfer mechanisms including conduction, convection, and radiation, 
presenting them in a visually engaging manner that changes color based on the temperature of the objects.

Features
Conduction: Simulate heat transfer through direct contact.
Convection: Model the transfer of heat through fluids and gases.
Radiation: Represent the emission of heat through electromagnetic waves.
Visual Representation: Visualize temperature changes using color gradients to represent varying intensities of heat.
How It Works
Shape Division
Objects are divided into a grid of small squares, allowing for detailed and localized temperature calculations. 
This division aids in accurately simulating how heat diffuses through different materials.

Heat Transfer Calculations
The engine calculates heat transfer between adjacent squares by considering factors such as temperature differences, 
the thermal conductivity of the material, and the simulation timestep.

Temperature Updates
Temperatures of individual squares are updated based on net heat gain or loss, integrating effects from conduction, convection, and radiation.

## Features

- **Dynamic Simulation**: Real-time simulation of heat transfer.
- **Customizable Materials**: Users can define the thermal properties of materials.
- **Interactive Visuals**: Real-time graphical representation of temperature changes.


## Installation MacOS
### 1. Clone the repository 
```bash
    git clone https://github.com/Exynos9820/ThedyxEngine
    cd ThedyxEngine 
```
### 2. .NET 9.0
Download and install the latest stable .NET 9.0 SDK & Runtime:  
- https://github.com/dotnet/core/blob/main/release-notes/9.0/9.0.1/9.0.1.md
### 3. Install MAUI Workload
```bash
sudo dotnet workload install maui
sudo dotnet workload restore
```
### 4 OS Setup
#### 4.1 macOS Setup
1) Install Xcode Command-Line Tools 
```bash
xcode-select --install
```
2) Set Xcode Developer Path
```bash
sudo xcode-select -s /Applications/Xcode.app/Contents/Developer
```
3) Accept Xcode License
```bash
sudo xcodebuild -license
```
4) Optional: Windows Interop Package If you encounter missing interop errors, add:
```bash
cd ThedyxEngine
dotnet add package Microsoft.Windows.CsWinRT --version 2.2.0
cd ..
```
#### 4.2 Windows Setup
1) Download and install the desktop runtime:
- https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-9.0.2-windows-x64-installer
2) Install Windows SDK
```bash
 winget install --id=Microsoft.WindowsSDK.10.0.19041 -e
```
3) Install Windows App SDK: 
- https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads
4) Add CsWinRT package:
```bash
cd ThedyxEngine
dotnet add package Microsoft.Windows.CsWinRT --version 2.2.0
cd ..
```

### 5. Build Project
```bash
dotnet build ThedyxEngine.sln
```


# Documentation

ThedyxEngine uses **Doxygen** to generate API reference from the source code
comments. You only need the *Doxyfile* in the repository root; everything else
is produced automatically.

## OS Setup
### Windows 
Download the doxygen-*.exe installer and (optionally) graphviz-*.exe from their official sites, then run them.
- https://www.doxygen.nl/download.html

### MacOS
Install Doxygen with brew
```bash
brew install doxygen graphviz
```

## Run doxygen
In a root directory, that contains Doxyfile, run command
```bash
doxygen
```
