using Microsoft.Maui.Controls.Shapes;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine;

public class EngineGrainLiquid : GrainSquare {
    public enum MaterialState {
        Liquid,
        Solid,
        Gas
    }
    
    private double AccumulatedEnergy { get; set; }
    
    public MaterialState CurrentMaterialState { get; set; }
    
    public EngineGrainLiquid(string name, Point position, Material material) : base(name, position) {
        Material = material;
        SetStateFromTemperature();
        SetCachedPoints();
    }
    
    
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
     * Generates the polygons that visually represent the square.
     * This method overrides the abstract method defined in \ref EngineObject.
     * \return List of polygons constituting the square's visual representation.
     */
    public override void GetPolygons(CanvasManager canvasManager, out List<RectF> rects, out List<double> temperatures, out List<float> opacities) {
        rects = [];
        temperatures = [];
        opacities = [];

        var rect = new RectF((float)_position.X, (float)_position.Y, (float)(_cachedPointB.X - _position.X), (float)(_cachedPointB.Y - _position.Y));
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

    public new void ApplyEnergyDelta() {
        lock (EnergyLock) {
            double tempDelta;
            if (CurrentMaterialState == MaterialState.Solid)
                tempDelta = EnergyDelta / Const.GridStep / Const.GridStep /
                               _material.SolidSpecificHeatCapacity / _material.SolidDensity;
            else if (CurrentMaterialState == MaterialState.Liquid)
                tempDelta = EnergyDelta / Const.GridStep / Const.GridStep /
                               _material.LiquidSpecificHeatCapacity / _material.LiquidDensity;
            else
                tempDelta = EnergyDelta / Const.GridStep / Const.GridStep /
                            _material.GasSpecificHeatCapacity / _material.GasDensity;
            
            if(tempDelta >= 10000)
                throw new Exception("Change in temperature is too high");
            
            if(Double.IsNaN(tempDelta))
                throw new Exception("temp delta is NaN");
            // we need to check if the temperature we are going to set is in the right range
            // 1) current State is solid
            if (CurrentMaterialState == MaterialState.Solid) {
                // check if we are going to melt the object
                if (_currentTemperature + tempDelta >= _material.MeltingTemperature) {
                    _currentTemperature = _material.MeltingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.MeltingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.SolidSpecificHeatCapacity * _material.SolidDensity;
                    // check if we have enough energy to melt the object
                    double energyToMelt = _material.MeltingEnergy * Const.GridStep * Const.GridStep * _material.SolidDensity;
                    if (AccumulatedEnergy >= energyToMelt) {
                        AccumulatedEnergy -= energyToMelt;
                        CurrentMaterialState = MaterialState.Liquid;
                        // transfer the rest of the energy to the liquid
                        _currentTemperature += AccumulatedEnergy / Const.GridStep / Const.GridStep / _material.LiquidSpecificHeatCapacity / _material.LiquidDensity;
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
                    AccumulatedEnergy += EnergyDelta - (_material.BoilingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.LiquidSpecificHeatCapacity * _material.LiquidDensity;
                    // check if we have enough energy to boil the object
                    double energyToBoil = _material.BoilingEnergy * Const.GridStep * Const.GridStep * _material.LiquidDensity;
                    if (AccumulatedEnergy >= energyToBoil) {
                        AccumulatedEnergy -= energyToBoil;
                        CurrentMaterialState = MaterialState.Gas;
                    }
                }
                // check if we are going to freeze the object
                else if (_currentTemperature + tempDelta <= _material.MeltingTemperature) {
                    _currentTemperature = _material.MeltingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.MeltingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.LiquidSpecificHeatCapacity * _material.LiquidDensity;
                    // check if we have enough energy to freeze the object
                    double energyToFreeze = _material.MeltingEnergy * Const.GridStep * Const.GridStep * _material.LiquidDensity;
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
                    AccumulatedEnergy += EnergyDelta - (_material.BoilingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.GasSpecificHeatCapacity * _material.GasDensity;
                    // check if we have enough energy to condense the object
                    double energyToCondense = _material.BoilingEnergy * Const.GridStep * Const.GridStep * _material.GasDensity;
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