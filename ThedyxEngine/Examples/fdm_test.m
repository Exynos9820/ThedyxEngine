% fdm_test.m
% Simulation of heat exchange between two aluminum cubes with non-uniform temperature distribution

% Parameters
h = 1000;              % Heat transfer coefficient [W/m²·K]
area = 0.01;           % Contact area per node [m²]
cube_length = 0.1;     % Cube size [m]
rho = 2700;            % Density [kg/m³]
c = 900;               % Specific heat capacity [J/kg·K]
mass = rho * cube_length^3;

% Divide each cube into a grid (nodes)
num_nodes = 10;  % Number of nodes along one dimension
node_length = cube_length / num_nodes;  % Length of each node in meters

% Initial temperatures [°C] for each node
T1 = ones(num_nodes, num_nodes, num_nodes) * 400;  % Cube 1 (hot)
T2 = ones(num_nodes, num_nodes, num_nodes) * 600;   % Cube 2 (cold)

% Time settings
dt = 0.2;              % Time step [s]
total_time = 300;      % Total time to simulate [s]
steps = total_time / dt;

% Arrays to store average temperatures over time
T1_avg_array = zeros(1, steps);
T2_avg_array = zeros(1, steps);

% Simulation loop
for i = 1:steps
    % Iterate over each node and calculate the heat transfer
    for x = 1:num_nodes
        for y = 1:num_nodes
            for z = 1:num_nodes
                % Find adjacent nodes (simple 6-connected neighborhood)
                % Note: Handle boundary conditions at edges
                neighbors = [
                    [x-1, y, z]; [x+1, y, z];  % x direction neighbors
                    [x, y-1, z]; [x, y+1, z];  % y direction neighbors
                    [x, y, z-1]; [x, y, z+1]   % z direction neighbors
                ];

                % Loop through neighbors and apply Newton's Law of Cooling
                for n = 1:size(neighbors, 1)
                    nx = neighbors(n, 1);
                    ny = neighbors(n, 2);
                    nz = neighbors(n, 3);

                    % Ensure neighbors are within bounds
                    if nx > 0 && nx <= num_nodes && ny > 0 && ny <= num_nodes && nz > 0 && nz <= num_nodes
                        % Calculate the temperature difference and heat transfer
                        delta_T = T1(x, y, z) - T2(nx, ny, nz);  % Temperature difference
                        q = h * area * delta_T;  % Heat transfer rate [W]

                        % Update temperatures using the heat transfer rate
                        dT1 = q * dt / (mass * c);
                        dT2 = q * dt / (mass * c);

                        % Update the temperatures of the cubes
                        T1(x, y, z) = T1(x, y, z) - dT1;
                        T2(nx, ny, nz) = T2(nx, ny, nz) + dT2;
                    end
                end
            end
        end
    end

    % Calculate average temperature for both cubes at each time step
    T1_avg_array(i) = mean(T1(:));
    T2_avg_array(i) = mean(T2(:));
end

% Time vector for plotting
time = (0:steps-1) * dt;



figure;

plot(time, T1_avg_array, "r-", time, T2_avg_array, "b-");

big = 18;                      % twice the usual size

xlabel("Time [s]",           "FontSize", big);
ylabel("Temperature [°C]",   "FontSize", big);
title("Heat exchange between two aluminum cubes", "FontSize", big);

lgd = legend("Cube A (hot)", "Cube B (cold)");
set(lgd, "FontSize", big);

set(gca, "FontSize", big);    % tick labels & axes numbers

grid on;
