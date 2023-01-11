#pragma warning disable SA1402
#nullable disable

using System.Text.Json.Serialization;

namespace Gomez.FactorioService.Models
{
    public class RootStats
    {
        [JsonPropertyName("stats")]
        public Stats Stats { get; set; }

        [JsonPropertyName("forceData")]
        public ForceData ForceData { get; set; }

        [JsonPropertyName("ticks_played")]
        public int TicksPlayed { get; set; }

        [JsonPropertyName("ticks_played_seconds")]
        public int TicksPlayedSeconds { get; set; }
    }

    public class Color
    {
        [JsonPropertyName("r")]
        public int R { get; set; }

        [JsonPropertyName("g")]
        public int G { get; set; }

        [JsonPropertyName("b")]
        public int B { get; set; }

        [JsonPropertyName("a")]
        public int A { get; set; }
    }

 // File may only contain a single type
    public class EntityBuildCountStatistics
    {
        [JsonPropertyName("input")]
        public Input Input { get; set; }

        [JsonPropertyName("output")]
        public Output Output { get; set; }
    }

    public class FluidProductionStatistics
    {
        [JsonPropertyName("input")]
        public Input Input { get; set; }

        [JsonPropertyName("output")]
        public Output Output { get; set; }
    }

    public class ForceData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public Color Color { get; set; }
    }

    public class Input
    {
        [JsonPropertyName("wooden-chest")]
        public int WoodenChest { get; set; }

        [JsonPropertyName("iron-chest")]
        public int IronChest { get; set; }

        [JsonPropertyName("transport-belt")]
        public int TransportBelt { get; set; }

        [JsonPropertyName("fast-transport-belt")]
        public int FastTransportBelt { get; set; }

        [JsonPropertyName("underground-belt")]
        public int UndergroundBelt { get; set; }

        [JsonPropertyName("splitter")]
        public int Splitter { get; set; }

        [JsonPropertyName("burner-inserter")]
        public int BurnerInserter { get; set; }

        [JsonPropertyName("inserter")]
        public int Inserter { get; set; }

        [JsonPropertyName("long-handed-inserter")]
        public int LongHandedInserter { get; set; }

        [JsonPropertyName("fast-inserter")]
        public int FastInserter { get; set; }

        [JsonPropertyName("filter-inserter")]
        public int FilterInserter { get; set; }

        [JsonPropertyName("small-electric-pole")]
        public int SmallElectricPole { get; set; }

        [JsonPropertyName("medium-electric-pole")]
        public int MediumElectricPole { get; set; }

        [JsonPropertyName("big-electric-pole")]
        public int BigElectricPole { get; set; }

        [JsonPropertyName("pipe")]
        public int Pipe { get; set; }

        [JsonPropertyName("pipe-to-ground")]
        public int PipeToGround { get; set; }

        [JsonPropertyName("small-lamp")]
        public int SmallLamp { get; set; }

        [JsonPropertyName("stone-brick")]
        public int StoneBrick { get; set; }

        [JsonPropertyName("repair-pack")]
        public int RepairPack { get; set; }

        [JsonPropertyName("boiler")]
        public int Boiler { get; set; }

        [JsonPropertyName("steam-engine")]
        public int SteamEngine { get; set; }

        [JsonPropertyName("burner-mining-drill")]
        public int BurnerMiningDrill { get; set; }

        [JsonPropertyName("electric-mining-drill")]
        public int ElectricMiningDrill { get; set; }

        [JsonPropertyName("offshore-pump")]
        public int OffshorePump { get; set; }

        [JsonPropertyName("stone-furnace")]
        public int StoneFurnace { get; set; }

        [JsonPropertyName("assembling-machine-1")]
        public int AssemblingMachine1 { get; set; }

        [JsonPropertyName("assembling-machine-2")]
        public int AssemblingMachine2 { get; set; }

        [JsonPropertyName("lab")]
        public int Lab { get; set; }

        [JsonPropertyName("coal")]
        public int Coal { get; set; }

        [JsonPropertyName("stone")]
        public int Stone { get; set; }

        [JsonPropertyName("iron-ore")]
        public int IronOre { get; set; }

        [JsonPropertyName("copper-ore")]
        public int CopperOre { get; set; }

        [JsonPropertyName("iron-plate")]
        public int IronPlate { get; set; }

        [JsonPropertyName("copper-plate")]
        public int CopperPlate { get; set; }

        [JsonPropertyName("steel-plate")]
        public int SteelPlate { get; set; }

        [JsonPropertyName("copper-cable")]
        public int CopperCable { get; set; }

        [JsonPropertyName("iron-stick")]
        public int IronStick { get; set; }

        [JsonPropertyName("iron-gear-wheel")]
        public int IronGearWheel { get; set; }

        [JsonPropertyName("electronic-circuit")]
        public int ElectronicCircuit { get; set; }

        [JsonPropertyName("automation-science-pack")]
        public int AutomationSciencePack { get; set; }

        [JsonPropertyName("logistic-science-pack")]
        public int LogisticSciencePack { get; set; }

        [JsonPropertyName("submachine-gun")]
        public int SubmachineGun { get; set; }

        [JsonPropertyName("firearm-magazine")]
        public int FirearmMagazine { get; set; }

        [JsonPropertyName("piercing-rounds-magazine")]
        public int PiercingRoundsMagazine { get; set; }

        [JsonPropertyName("shotgun-shell")]
        public int ShotgunShell { get; set; }

        [JsonPropertyName("light-armor")]
        public int LightArmor { get; set; }

        [JsonPropertyName("heavy-armor")]
        public int HeavyArmor { get; set; }

        [JsonPropertyName("stone-wall")]
        public int StoneWall { get; set; }

        [JsonPropertyName("gate")]
        public int Gate { get; set; }

        [JsonPropertyName("gun-turret")]
        public int GunTurret { get; set; }

        [JsonPropertyName("radar")]
        public int Radar { get; set; }

        [JsonPropertyName("water")]
        public double Water { get; set; }

        [JsonPropertyName("steam")]
        public double Steam { get; set; }

        [JsonPropertyName("small-biter")]
        public int SmallBiter { get; set; }
    }

    public class ItemProductionStatistics
    {
        [JsonPropertyName("input")]
        public Input Input { get; set; }

        [JsonPropertyName("output")]
        public Output Output { get; set; }
    }

    public class KillCountStatistics
    {
        [JsonPropertyName("input")]
        public Input Input { get; set; }

        [JsonPropertyName("output")]
        public Output Output { get; set; }
    }

    public class Output
    {
        [JsonPropertyName("transport-belt")]
        public int TransportBelt { get; set; }

        [JsonPropertyName("inserter")]
        public int Inserter { get; set; }

        [JsonPropertyName("fast-inserter")]
        public int FastInserter { get; set; }

        [JsonPropertyName("pipe")]
        public int Pipe { get; set; }

        [JsonPropertyName("stone-brick")]
        public int StoneBrick { get; set; }

        [JsonPropertyName("stone-furnace")]
        public int StoneFurnace { get; set; }

        [JsonPropertyName("assembling-machine-1")]
        public int AssemblingMachine1 { get; set; }

        [JsonPropertyName("wood")]
        public int Wood { get; set; }

        [JsonPropertyName("coal")]
        public int Coal { get; set; }

        [JsonPropertyName("stone")]
        public int Stone { get; set; }

        [JsonPropertyName("iron-ore")]
        public int IronOre { get; set; }

        [JsonPropertyName("copper-ore")]
        public int CopperOre { get; set; }

        [JsonPropertyName("iron-plate")]
        public int IronPlate { get; set; }

        [JsonPropertyName("copper-plate")]
        public int CopperPlate { get; set; }

        [JsonPropertyName("steel-plate")]
        public int SteelPlate { get; set; }

        [JsonPropertyName("copper-cable")]
        public int CopperCable { get; set; }

        [JsonPropertyName("iron-stick")]
        public int IronStick { get; set; }

        [JsonPropertyName("iron-gear-wheel")]
        public int IronGearWheel { get; set; }

        [JsonPropertyName("electronic-circuit")]
        public int ElectronicCircuit { get; set; }

        [JsonPropertyName("automation-science-pack")]
        public int AutomationSciencePack { get; set; }

        [JsonPropertyName("logistic-science-pack")]
        public int LogisticSciencePack { get; set; }

        [JsonPropertyName("firearm-magazine")]
        public int FirearmMagazine { get; set; }

        [JsonPropertyName("piercing-rounds-magazine")]
        public int PiercingRoundsMagazine { get; set; }

        [JsonPropertyName("stone-wall")]
        public int StoneWall { get; set; }

        [JsonPropertyName("water")]
        public double Water { get; set; }

        [JsonPropertyName("steam")]
        public double Steam { get; set; }

        [JsonPropertyName("electric-mining-drill")]
        public int ElectricMiningDrill { get; set; }

        [JsonPropertyName("gun-turret")]
        public int GunTurret { get; set; }

        [JsonPropertyName("radar")]
        public int Radar { get; set; }

        [JsonPropertyName("small-electric-pole")]
        public int SmallElectricPole { get; set; }

        [JsonPropertyName("assembling-machine-2")]
        public int AssemblingMachine2 { get; set; }

        [JsonPropertyName("burner-inserter")]
        public int BurnerInserter { get; set; }

        [JsonPropertyName("burner-mining-drill")]
        public int BurnerMiningDrill { get; set; }

        [JsonPropertyName("gate")]
        public int Gate { get; set; }

        [JsonPropertyName("iron-chest")]
        public int IronChest { get; set; }

        [JsonPropertyName("item-on-ground")]
        public int ItemOnGround { get; set; }

        [JsonPropertyName("lab")]
        public int Lab { get; set; }

        [JsonPropertyName("long-handed-inserter")]
        public int LongHandedInserter { get; set; }

        [JsonPropertyName("small-lamp")]
        public int SmallLamp { get; set; }

        [JsonPropertyName("splitter")]
        public int Splitter { get; set; }

        [JsonPropertyName("underground-belt")]
        public int UndergroundBelt { get; set; }

        [JsonPropertyName("tree-09-brown")]
        public int Tree09Brown { get; set; }

        [JsonPropertyName("tree-09-red")]
        public int Tree09Red { get; set; }

        [JsonPropertyName("rock-big")]
        public int RockBig { get; set; }
    }

    public class Stats
    {
        [JsonPropertyName("item_production_statistics")]
        public ItemProductionStatistics ItemProductionStatistics { get; set; }

        [JsonPropertyName("fluid_production_statistics")]
        public FluidProductionStatistics FluidProductionStatistics { get; set; }

        [JsonPropertyName("kill_count_statistics")]
        public KillCountStatistics KillCountStatistics { get; set; }

        [JsonPropertyName("entity_build_count_statistics")]
        public EntityBuildCountStatistics EntityBuildCountStatistics { get; set; }
    }
}
