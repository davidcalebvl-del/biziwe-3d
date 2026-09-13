using UnityEngine;

namespace Biziwe.Vehicle
{
    public enum VehicleCategory
    {
        SportsCar, Supercar, Sedan, SUV, PickupTruck, Van, Truck,
        PoliceCar, Taxi, Bus, Motorcycle, DirtBike
    }

    /// <summary>
    /// Handling stats for one vehicle type/category — data-only, same pattern
    /// as MissionData and AmbientLines. Create one asset per vehicle, assign
    /// to VehicleController.stats, and that vehicle drives distinctly
    /// (a supercar isn't a pickup truck) without any code changes.
    ///
    /// SETUP:
    /// 1. Right-click in Project window > Create > Biziwe > Vehicle Stats.
    /// 2. Fill in values matching the vehicle's category — see the suggested
    ///    ranges in the tooltip comments below as a starting point.
    /// 3. Assign the asset to that vehicle prefab's VehicleController.stats field.
    /// </summary>
    [CreateAssetMenu(fileName = "NewVehicleStats", menuName = "Biziwe/Vehicle Stats")]
    public class VehicleStats : ScriptableObject
    {
        public string vehicleName = "Sedan";
        public VehicleCategory category = VehicleCategory.Sedan;

        [Header("Handling")]
        [Tooltip("Supercar ~3000, Sedan ~1500, Truck ~600, Motorcycle ~350")]
        public float maxMotorTorque = 1500f;
        [Tooltip("Supercar ~35, Sedan ~30, Truck ~22, Motorcycle ~40 (tighter turning)")]
        public float maxSteerAngle = 30f;
        [Tooltip("Supercar ~4000, Sedan ~3000, Truck ~5000 (heavier, needs more to stop), Motorcycle ~1500")]
        public float brakeTorque = 3000f;

        [Header("Physical")]
        [Tooltip("Supercar ~1300, Sedan ~1400, SUV ~2000, Truck ~4500, Motorcycle ~200")]
        public float mass = 1400f;

        [Header("Vehicle Health (see VehicleHealth.cs)")]
        [Tooltip("Motorcycles are fragile (~80), trucks are tanky (~400)")]
        public float maxHealth = 200f;
    }
}
