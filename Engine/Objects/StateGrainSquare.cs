using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine;

/**
 * \class StateGrainSquare
 * \brief Represents a square of state in the simulation.
 *
 * The StateGrainSquare class represents a square of state in the simulation.
 * The difference with GrainSquare that the aggregate state of the square can be changed.
 */
public class StateGrainSquare : GrainSquare {
    
    /**
     * \brief Material state of the square.
     */
    public enum MaterialState {
        Liquid,
        Solid,
        Gas
    }
    
    /** Accumulated energy between different states */
    private double AccumulatedEnergy { get; set; }
    
    /** Current aggregate state of the square */
    
    public MaterialState CurrentMaterialState { get; set; }
    
    /**
     * \brief Constructor for creating a new StateGrainSquare.
     * \param name The name of the engine object.
     * \param position The position of the square.
     * \param material The material of the square.
     */
    public StateGrainSquare(string name, Point position, Material material) : base(name, position) {
        Material = material;
        SetStateFromTemperature();
        SetCachedPoints();
    }
    
    /**
     * \brief Sets the state of the square based on the current temperature.
     */
    public void SetStateFromTemperature() {
        if(_currentTemperature < Material.MeltingTemperature) {
            CurrentMaterialState = MaterialState.Solid;
        } else if(_currentTemperature > Material.BoilingTemperature) {
            CurrentMaterialState = MaterialState.Gas;
        } else {
            CurrentMaterialState = MaterialState.Liquid;
        }
    }
    
    /**
     * Serializes the state grain square to a JSON representation.
     * \return A JSON string representing the state grain square.
     */
    public override string GetJsonRepresentation() {
        var settings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(new {
            Type = ObjectType.StateGrainSquare.ToString(),
            Name,
            Position = _position,
            SimulationTemperature = _simulationTemperature,
            MaterialName = _material.Name,
            IsGasStateAllowed = _isGasStateAllowed
        }, settings);
    }
    
    /**
     * Generates the polygons that visually represent the square.
     * This method overrides the abstract method defined in \ref EngineObject.
     * \return List of polygons constituting the square's visual representation.
     */
    public override void GetPolygons(CanvasManager canvasManager, out List<RectF> rects, out List<double> temperatures, out List<float> opacities) {
        rects = [];
        temperatures = [];
        opacities = [];

        var rect = new RectF((float)_position.X, (float)_position.Y, 1, 1);
        switch (CurrentMaterialState) {
            case MaterialState.Solid:
                opacities.Add(1);
                break;
            case MaterialState.Liquid:
                opacities.Add(0.5f);
                break;
            default:
                opacities.Add(0.1f);
                break;
        }

        rects.Add(rect);
        temperatures.Add(_currentTemperature);
    }

    /**
     * \brief Gets the emissivity of the material based on the current state.
     * \return The emissivity of the material.
     */
    public double GetMaterialEmissivity() {
        // check for state of the object and get the right emissivity
        if (CurrentMaterialState == MaterialState.Solid) {
            return Material.SolidEmissivity;
        } else if (CurrentMaterialState == MaterialState.Liquid) {
            return Material.LiquidEmissivity;
        } else {
            return Material.GasEmissivity;
        }
    }
    
    /**
     * \brief Gets the specific heat capacity of the material based on the current state.
     * \return The specific heat capacity of the material.
     */
    public double GetMaterialThermalConductivity() {
        // check for state of the object and get the right thermal conductivity
        if (CurrentMaterialState == MaterialState.Solid) {
            return Material.SolidThermalConductivity;
        } else if (CurrentMaterialState == MaterialState.Liquid) {
            return Material.LiquidThermalConductivity;
        } else {
            return Material.GasThermalConductivity;
        }
    }

    /**
     * \brief Applies the energy delta to the square.
     * It checks if we are going to change aggregate state of the square.
     * If no, it just changes the temperature and/or accumulates the energy.
     */
    public new void ApplyEnergyDelta() {
        if(_isTemperatureFixed) return;
        lock (EnergyLock) {
            double tempDelta;
            if (CurrentMaterialState == MaterialState.Solid)
                tempDelta = EnergyDelta / GlobalVariables.GridStep / GlobalVariables.GridStep /
                               _material.SolidSpecificHeatCapacity / _material.SolidDensity;
            else if (CurrentMaterialState == MaterialState.Liquid)
                tempDelta = EnergyDelta / GlobalVariables.GridStep / GlobalVariables.GridStep /
                               _material.LiquidSpecificHeatCapacity / _material.LiquidDensity;
            else
                tempDelta = EnergyDelta / GlobalVariables.GridStep / GlobalVariables.GridStep /
                            _material.GasSpecificHeatCapacity / _material.GasDensity;
            
            if(tempDelta >= 10000)
                throw new Exception($"Change in temperature is too high. Happened for {Name}, energy delta {EnergyDelta}, material state {CurrentMaterialState}, Material: {_material.Name}\"");
            
            if(Double.IsNaN(tempDelta))
                throw new Exception($"temp delta is NaN. Happened for {Name}, energy delta {EnergyDelta}, material state {CurrentMaterialState}, Material: {_material.Name}");
            // we need to check if the temperature we are going to set is in the right range
            // 1) current State is solid
            if (CurrentMaterialState == MaterialState.Solid) {
                // check if we are going to melt the object
                if (_currentTemperature + tempDelta >= _material.MeltingTemperature) {
                    _currentTemperature = _material.MeltingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.MeltingTemperature - _currentTemperature) * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.SolidSpecificHeatCapacity * _material.SolidDensity;
                    // check if we have enough energy to melt the object
                    double energyToMelt = _material.MeltingEnergy * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.SolidDensity;
                    if (AccumulatedEnergy >= energyToMelt) {
                        AccumulatedEnergy -= energyToMelt;
                        CurrentMaterialState = MaterialState.Liquid;
                        // transfer the rest of the energy to the liquid
                        _currentTemperature += AccumulatedEnergy / GlobalVariables.GridStep / GlobalVariables.GridStep / _material.LiquidSpecificHeatCapacity / _material.LiquidDensity;
                        AccumulatedEnergy = 0;
                    }
                }else {
                    _currentTemperature += tempDelta;
                }
            }else if (CurrentMaterialState == MaterialState.Liquid) {
                // check if we are going to boil the object
                if (_currentTemperature + tempDelta >= _material.BoilingTemperature) {
                    _currentTemperature = _material.BoilingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.BoilingTemperature - _currentTemperature) * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.LiquidSpecificHeatCapacity * _material.LiquidDensity;
                    // check if we have enough energy to boil the object
                    double energyToBoil = _material.BoilingEnergy * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.LiquidDensity;
                    if (AccumulatedEnergy >= energyToBoil && _isGasStateAllowed) {
                        AccumulatedEnergy -= energyToBoil;
                        CurrentMaterialState = MaterialState.Gas;
                    }
                }
                // check if we are going to freeze the object
                else if (_currentTemperature + tempDelta <= _material.MeltingTemperature) {
                    _currentTemperature = _material.MeltingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.MeltingTemperature - _currentTemperature) * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.LiquidSpecificHeatCapacity * _material.LiquidDensity;
                    // check if we have enough energy to freeze the object
                    double energyToFreeze = _material.MeltingEnergy * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.LiquidDensity;
                    if (AccumulatedEnergy >= energyToFreeze) {
                        AccumulatedEnergy -= energyToFreeze;
                        CurrentMaterialState = MaterialState.Solid;
                    }
                }
                else {
                    // just add temperature
                    _currentTemperature += tempDelta;
                }
            }else if (CurrentMaterialState == MaterialState.Gas) {
                // check if we are going to condense the object
                if (_currentTemperature + tempDelta <= _material.BoilingTemperature) {
                    _currentTemperature = _material.BoilingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.BoilingTemperature - _currentTemperature) * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.GasSpecificHeatCapacity * _material.GasDensity;
                    // check if we have enough energy to condense the object
                    double energyToCondense = _material.BoilingEnergy * GlobalVariables.GridStep * GlobalVariables.GridStep * _material.GasDensity;
                    if (AccumulatedEnergy >= energyToCondense) {
                        AccumulatedEnergy -= energyToCondense;
                        CurrentMaterialState = MaterialState.Liquid;
                    }
                }else {
                    _currentTemperature += tempDelta;
                }
            }
            CurrentTemperature = Math.Max(0, CurrentTemperature);
            if (CurrentTemperature >= 10000) {
                throw new Exception("temperature is too high");
            }
            EnergyDelta = 0;
        }
    }
}