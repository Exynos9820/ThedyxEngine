  # Heat Transfer Calculations

## Overview
This project involves detailed calculations of heat transfer through radiation. The objects involved in this simulation are assumed to have unique properties such as names for identification and uniform dimensions (width of 1 mm and each cell measuring 1 mm by 1 mm).

## Multi-threaded Calculation Strategy
- **Grouping**: Objects are divided into groups based on the number of processor threads available.
- **Parallel Processing**: Each group is processed in a separate thread to optimize performance and computation speed.

## Radiation Heat Transfer

### Conduction Calculation
Fourier's Law of Heat Conduction is used to calculate the heat emitted by an object:
P = -x * S * dT /l
S = l for this 2D simulation, because it's a problem of an method of end bodies

So the heat emitted by an object can be calculated as:
P = -x * dT
So dQ = -x * dT * dt

So dT = -x * dT * dt / (c * m)
https://ru.wikipedia.org/wiki/%D0%A2%D0%B5%D0%BF%D0%BB%D0%BE%D0%BF%D1%80%D0%BE%D0%B2%D0%BE%D0%B4%D0%BD%D0%BE%D1%81%D1%82%D1%8C#%D0%9A%D0%BE%D1%8D%D1%84%D1%84%D0%B8%D1%86%D0%B8%D0%B5%D0%BD%D1%82_%D1%82%D0%B5%D0%BF%D0%BB%D0%BE%D0%BF%D1%80%D0%BE%D0%B2%D0%BE%D0%B4%D0%BD%D0%BE%D1%81%D1%82%D0%B8

### Radiative Heat Exchange Between Two Objects
For two objects exchanging heat through radiation, assuming no intervening medium absorbs the radiation, the heat transfer can be modeled using the following equation for gray bodies:

\[ Q = \sigma \cdot A \cdot \frac{e_1 \cdot e_2}{e_1 + e_2 - e_1 \cdot e_2} \cdot (T_2^4 - T_1^4) \]

Where:
- \( e_1 \) and \( e_2 \) are the emissivities of the two objects,
- \( T_1 \) and \( T_2 \) are their respective temperatures.

## Temperature Representation
- Temperatures are represented using color codes:
  - **Violet**: 0 degrees (as a baseline temperature).
  - **Red**: Represents temperatures of 200 degrees and above.

These calculations form the core of the thermal analysis in this project, aiming to provide accurate predictions of heat interactions under various conditions.
