# ThempoEngine Documentation

Welcome to the documentation of ThempoEngine, a 2D physics engine designed to simulate heat transfer across different materials using a visually intuitive approach.

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
\
