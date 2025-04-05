# ThedyxEngine Documentation

Welcome to the documentation of ThedyxEngine, a 2D physics engine designed to simulate heat transfer across different materials using a visually intuitive approach.

## Overview

TempoEngine is a 2D physics engine designed to simulate heat transfer across different materials using a visually intuitive approach. 
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
1) Clone the repository
2) Download and install Dotnet9 https://github.com/dotnet/core/blob/main/release-notes/9.0/9.0.1/9.0.1.md
3) dotnet workload install MAUI in the terminal
4) dotnet add package Microsoft.Windows.CsWinRT --version 2.2.0 (sometimes)
5) dotnet build ThedyxEngine.sln

## Installation Windows
1) Clone the repository
2) Download and install Dotnet9 https://github.com/dotnet/core/blob/main/release-notes/9.0/9.0.1/9.0.1.md
3) https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-9.0.2-windows-x64-installer
4) dotnet workload install MAUI in the terminal
5) Download and install https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads
6) dotnet add package Microsoft.Windows.CsWinRT --version 2.2.0
7) dotnet build ThedyxEngine.sln
