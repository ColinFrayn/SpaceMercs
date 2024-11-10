using OpenTK.Mathematics;
using System.Globalization;

namespace SpaceMercs {
    public static class Const {
        // Version
        public const string strVersion = "1.0";

        // ---=== DEBUG MODE SETTINGS ===---
        public const float DEBUG_ENCOUNTER_FREQ_MOD = 1;  // Modification factor for encounter frequency. (multiplier, default = 1, <=0 turns off all encounters)
        public const bool DEBUG_ALL_ENCOUNTERS_INACTIVE = false;  // All ship encounters when travelling are resolved as inactive (repair/salvage)
        public const bool DEBUG_MORE_BOARDERS = false; // Make it much more likely hostile vessels will board
        public const bool DEBUG_VIEW_ALL_CIVS = false;  // Set starting systems for all civs = visited
        public const bool DEBUG_VISIBLE_ALL = false; // Can see the entire map when in a mission
        public const bool DEBUG_RANDOMISE_VENDORS = true;  // Provide a button to completely regenerate all items/mercs/missions at a colony
        public const bool DEBUG_SHOW_SELECTED_ENTITY_VIS = false;  // Show visibility of selected entity with dots
        public const int DEBUG_WEAPON_SKILL_MOD = 1;  // Weapon skill experience multiplier
        public const int DEBUG_EXPERIENCE_MOD = 1;  // Experience accrual multiplier
        public const int DEBUG_ADDITIONAL_STARTING_CASH = 0; // More cash at the start

        // Character settings
        public const int SpareAttributePoints = 0; // Starting pot of skill points
        public const int MaximumSkillDeficit = 5; // A skill can't be lowered more than this many points beneath racial average
        public const double MovementCost = 2.0;  // Cost, in stamina points, for moving one square
        public const double MeleeCost = 6.0;  // Cost, in stamina points, for a melee attack
        public const double SearchCost = 10.0; // Cost, in stamina points, for searching the nearby area
        public const double UseItemCost = 10.0; // Cost, in stamina points, for using an item
        public const int BaseSearchRadius = 5; // Maximum radius of any search for unskilled searcher (increase by 1 per point in perception)
        public const int PassiveSearchRadius = 2; // Maximum radius of any search for unskilled passive searcher (increase by 1 per point in perception)
        public const double BaseSearchChance = 70.0; // Chance of spotting a hidden object at distance zero (plus Insight)
        public const double PassiveSearchChance = 30.0; // Chance of spotting a hidden object at distance zero (plus Insight)
        public const double SearchReduction = 8.0; // Reduction in chance to spot an object for each metre distant from the searcher
        public const double SearchBoostPerSkill = 2.0; // Increase in chance to spot a hidden object for each point in Perception.
        public const double EncumbranceSearchPenalty = 20.0; // Penalty (proportionately applied) for searching when 100% encumbered.
        public const double PerceptionSearchRadiusBoost = 0.5; // Increase in search radius for every point of perception
        public const double MissionDifficultySearchScale = 2.0; // Every extra diff in mission reduces search chance by this.
        public const double SkillConstructChanceModifier = 5.0; // Bonus chance to construct an item per level of skill
        public const int WeaponSkillBase = 25; // Skill required to increase a weapon skill from zero to one. 1-2 requires another twice this, 2-3 requires another 3x etc.
        public const double EquipmentLevelCostExponent = 1.35;  // For each level the weapon quality improves, this is the power law relationship to cost
        public const double EquipmentLevelCostBaseExponent = 1.0;  // For each level the weapon quality improves, additionally multiply cost by this factor 
        public const double EquipmentLevelRarityScale = 0.2;  // Each level of equipment quality makes it this much rarer
        public const double MaxCarryScale = 0.6; // Affects the maximum amount soldiers can carry before encumbrance affects their stamina regen rate
        public const double ArmourCostExponent = 4.0; // Affects cost scaling for armour based on how much better it is than the base
        public const double ShieldCostExponent = 4.0; // Affects cost scaling for armour based on how much better the shields are than the base
        public const double MassCostExponent = 1.5; // Affects cost scaling for armour based on how much lighter it is than the base
        public const double BonusPhysicalArmourValue = 0.03; // Percent bonus to the cost for each single point of bonus armour for physical damage
        public const double BonusOtherArmourValue = 0.01; // Percent bonus to the cost for each single point of bonus armour for any other damage type
        public const double ModificationCost = 20.0; // Cost for a modification for disposable materials

        // Solar system parameters
        public const double MoonRadius = 1400000.0;
        public const double MoonRadiusSigma = 200000.0;
        public const double MoonRadiusMin = 400000.0;
        public const double MoonOrbit = 4E8;
        public const double MoonOrbitSigma = 5E7;
        public const double PlanetOrbit = 4E10;
        public const double PlanetOrbitFactor = 1.4;
        public const double PlanetOrbitFactorSigma = 0.02;
        public const double PlanetSize = 6000000.0;
        public const double PlanetSizeSigma = PlanetSize / 6;
        public const double PlanetSizeMin = PlanetSize / 4;
        public const double GasGiantScale = 10.0;
        public const double GasGiantScaleSigma = 1.0;
        public const int MaxPlanetsPerSystem = 9;
        public const double StarRotation = 25.0 * 24.0 * 3600.0;
        public const double StarRotationSigma = StarRotation / 10.0;
        public const double StarRotationMin = StarRotation / 5.0;
        public const double EarthOrbitalPeriod = 1.0 * 24.0 * 3600.0 * 365.0;
        public const double EarthOrbitalPeriodSigma = EarthOrbitalPeriod / 15.0;
        public const double MoonOrbitalPeriod = 1.0 * 24.0 * 3600.0 * 27.32; // Sidereal month
        public const double MoonOrbitalPeriodSigma = MoonOrbitalPeriod / 15.0;
        public const double DayLength = 24.0 * 3600.0;
        public const double DayLengthSigma = DayLength / 10.0;

        // Planet generation
        public static Vector3 PlanetTypeToCol1(Planet.PlanetType pt) {
            switch (pt) {
                case Planet.PlanetType.Desert: return new Vector3(0.4f, 0.3f, 0);
                case Planet.PlanetType.Gas: return new Vector3(0.5f, 0.5f, 0.6f);
                case Planet.PlanetType.Oceanic: return new Vector3(0.0f, 0.0f, 0.4f);
                case Planet.PlanetType.Ice: return new Vector3(0.0f, 0.0f, 0.5f);
                case Planet.PlanetType.Rocky: return new Vector3(0.3f, 0.3f, 0.3f);
                case Planet.PlanetType.Volcanic: return new Vector3(0.3f, 0.0f, 0.0f);
            }
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        public static Vector3 PlanetTypeToCol2(Planet.PlanetType pt) {
            switch (pt) {
                case Planet.PlanetType.Desert: return new Vector3(0.65f, 0.65f, 0.0f);
                case Planet.PlanetType.Gas: return new Vector3(0.6f, 0.6f, 0.8f);
                case Planet.PlanetType.Oceanic: return new Vector3(0.2f, 0.4f, 1.0f);
                case Planet.PlanetType.Ice: return new Vector3(0.0f, 0.1f, 1.0f);
                case Planet.PlanetType.Rocky: return new Vector3(0.5f, 0.5f, 0.5f);
                case Planet.PlanetType.Volcanic: return new Vector3(0.7f, 0.0f, 0.0f);
            }
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        public static Vector3 PlanetTypeToCol3(Planet.PlanetType pt) {
            switch (pt) {
                case Planet.PlanetType.Desert: return new Vector3(0.9f, 0.8f, 0.0f);
                case Planet.PlanetType.Gas: return new Vector3(0.7f, 0.7f, 0.9f);
                case Planet.PlanetType.Oceanic: return new Vector3(0.0f, 1.0f, 0.0f);
                case Planet.PlanetType.Ice: return new Vector3(0.0f, 0.2f, 1.0f);
                case Planet.PlanetType.Rocky: return new Vector3(0.5f, 0.5f, 0.5f);
                case Planet.PlanetType.Volcanic: return new Vector3(1.0f, 0.0f, 0.0f);
            }
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        public static Vector3 PlanetTypeToCol4(Planet.PlanetType pt) {
            switch (pt) {
                case Planet.PlanetType.Desert: return new Vector3(1.0f, 1.0f, 0.3f);
                case Planet.PlanetType.Gas: return new Vector3(0.8f, 0.8f, 1.0f);
                case Planet.PlanetType.Oceanic: return new Vector3(0.6f, 1.0f, 0.8f);
                case Planet.PlanetType.Ice: return new Vector3(0.8f, 1.0f, 1.0f);
                case Planet.PlanetType.Rocky: return new Vector3(0.75f, 0.75f, 0.75f);
                case Planet.PlanetType.Volcanic: return new Vector3(1.0f, 0.8f, 0.0f);
            }
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        public const double TerrainScale = 40.0;
        public const int PerlinOctaves = 7;
        public const int SeedBuffer = 50;
        public const int MaxRings = 4;
        public const double TempTolerance = 10.0; // Temperature tolerance for a home system
        public const int MinPlanetDensity = 1;
        public const int MaxPlanetDensity = 10;
        public const int DefaultPlanetDensity = 5;

        // Galaxy settings
        public const int SectorSize = 10; // Light years on a side
        public const int MinStarsPerSector = 8;
        public const int MaxStarsPerSector = 22;
        public const int DefaultStarsPerSector = 14;
        public const double MinStarDistance = 1.1; // Light years

        // Combat settings
        public const double ArmourScale = 30.0; // The higher this is, the less effective armour is at mitigating damage.
        public const double CreatureMeleeDamageScale = 0.40;  // The higher this is, the more damage creatures do when attacking in melee without a weapon
        public const double CreatureLevelAttackStep = 0.15;  // %age growth in creature attack each level (over base)
        public const double CreatureLevelDefenceStep = 0.15;  // %age growth in creature defence each level (over base)
        public const double CreatureLevelHealthStep = 0.25;  // %age growth in creature health each level (over base)
        public const double CreatureLevelShieldsStep = 0.2;  // %age growth in creature shields each level (over base)
        public const double CreatureLevelArmourStep = 0.15;  // %age growth in creature armour each level (over base)
        public const double CreatureLevelStaminaStep = 1.0;  // Absolute growth in creature stamina each level
        public const double HitBias = 4.0; // Bias towards hitting when attacking
        public const double SoldierHitBias = 0.0; // Bias towards a player-controlled soldier hitting when attacking
        public const double RandomHitScale = 10.0; // Add extra randomness to help cancel out "certain hit" or "certain miss"
        public const double AttackScale = 0.8; // Bias towards attack score
        public const double DefenceScale = 0.8; // Bias towards defence score
        public const double GuaranteedHitScale = 0.5;  // The higher this is, the higher the weighting for absolute att-def, leading to more predictable hits/misses
        public const double EncumbranceHitPenalty = 4.0; // When fully encumbered, reduce hit rolls by this much (or proportionately).
        public const double HitSizePowerLaw = 2.0;  // Increase chance to hit for larger creatures
        public const double PartialCoverDefenceBonus = 2.5; // When hiding by a wall from perspective of the (ranged) attacker, increase defence by this much
        public const double BaseDetectionRange = 7.0; // Range in squares at which a creature of same level can spot you, if you're unencumbered and with default agility
        public const int CreatureAlertWarningDistance = 4;  // Range to which any alerted creature can trigger other creatures that they can see to also be alert
        public const double FireWeaponExtraDetectionRange = 4.0; // If a soldier fires his weapon then alert all entities no more than this distance outside his detectionr ange
        public const double CreatureExperienceScale = 0.7;  // Scale creature experience value by this amount
        public const double TrapDamageScale = 1.5;  // Increase damage done by traps
        public const double CreatureAttackDamageScale = 0.06;  // Modifier applied to damage done by Creatures
        public const double CreatureAttackDamageBaseMod = 0.7; // Scale factor for zeroth level creatures damage.
        public const double SoldierAttackDamageScale = 0.1;  // Modifier applied to damage done by player-controlled Soldiers
        public const double TurnLength = 10.0; // Length of one combat turn in seconds
        public const double SharpshooterRangeMod = 0.85; // Snipers multiply drop-off penalty by this factor per sniper level.
        public const double SurpriseHitMod = 5.0; // Hit bonus if the target is not alert to your presence

        // Miscellaneous
        public const double InitialCash = 50.0;
        public const int MaxItemLevel = 5;
        public const int SoldierLevelExperience = 1000; // Experience for level 1->2
        public const double SoldierLevelExponent = 2.0;  // Each level gets this many times further apart
        public const double SoldierLevelScale = 1.5; // Experience scale
        public const int ItemIDBase = 2000000; // ID of first ItemType
        public const int NextThingIDBase = 3000000; // ID of first NextThing (whatever that might be). Give sufficient space between them
        public const double BaseEncounterScarcity = 1500.0;  // Base chance for an encounter when travelling (higher = less frequent)
        public const double InterstellarTravelSafetyBonus = 15.0; // Less likely to get intercepted in interstellar space
        public const double BaseInterceptionChance = 40.0; // Base scaling for encountering something during a journey. The higher the more often and the more dangerous.
        public const double EncounterLevelScalingInnerRadius = 5.0; // Size of the inner scaling region, in ly.
        public const double EncounterLevelScalingDistanceInner = 1.0; // Within the inner scaling region, this is the base difficulty increase scale length.
        public const double EncounterLevelScalingExponentInner = 1.0; // 1 means encounter difficulty scales linearly. 0 means no scaling. 0.5 = sqrt, etc.
        public const double EncounterLevelScalingDistanceOuter = 2.8; // Outside of the inner scaling region, this is the base difficulty increase scale length.
        public const double EncounterLevelScalingExponentOuter = 0.75; // 1 means encounter difficulty scales linearly. 0 means no scaling. 0.5 = sqrt, etc.
        public const double EncounterFreqScale = 0.15; // The lower this is (>0.0) the less frequently we encounter anything when travelling (active or passive).
        public const double ShipBountyScale = 0.15;  // Scale for calculating bounty of enemy ships defeated
        public const double HyperspaceGateTimeFactor = 20.0; // It takes this factor less time to travel by hyperspace gate than travel at light speed.
        public const double HyperspaceCostScale = 15.0; // Divide the cost of the hyperspace travel by this factor
        public const double HyperspaceCostDistanceExponent = 1.1; // Exponent for distance in Ly when calculating cost. 1 = linear.
        public const double HyperspaceCostHullExponent = 1.2; // Exponent for hull size when calculating cost. 1 = linear.
        public const int InitialCivilisationSize = 20; // Total population of all civs at the start
        public const double BasicTradeRouteLength = 4.0; // Maximum length of a trade route in light years for a basic colony
        public const double MaxTradeRouteLength = 7.0; // Maximum length of a trade route in light years for a large colony
        public const double DailyResearchProb = 0.002; // Chance of successfully researching a technology, per day, for a nonHuman race.

        // Colony stuff
        public const int MaxColonyMercenaries = 16;  // In a colony
        public const int MaxColonyMissions = 16;  // In a colony
        public const double TradeRouteColonyGrowthRate = 0.75; // Colonies grow more quickly when system has trade route(s) (modify delay by this amount)
        public const double UnconnectedColonyCostMod = 1.5; // Everything is more expensive (and valuable) in distant systems without trade routes
        public const int LongEnoughGapToResetColonyInventory = 365; // Number of days after which we just throw away the old inventory
        public const double MercenaryCostScale = 4.0;  // Base price scale of a mercenary, not including kit
        public const double MercenaryCostBase = 1.1;  // Base for exponential price calculation for a mercenary
        public const double MercenaryCostExponent = 1.4;  // Exponent scale for exponential price calculation for a mercenary
        public const double MercenaryKitValueScale = 0.8; // Mercenary kit is discounted by this amount
        public const int MerchantStockResetDuration = 50;  // When completely resettign a merchant's store, how many days worth of incoming stock do you generate?
        public const double UpgradeCostModifier = 1.2; // To upgrade an item by one level
        public const double SellDiscount = 0.5; // Get this fraction back when selling an item
        public const double ColonySeedRate = 5.0; // Affects the speed at which new colonies are created in populated systems.
        public const double ColonySeedTarget = 100.0; // Target seeding point at which level the colony may seed

        // Race relations
        public const int InitialColonyCount = 3;   // When starting up, how many colonies to add to a system
        public const int HomeSysColonyCount = 3;   // Extra colonies for home system
        public const int RaceRelationsLevelToAllowSpecialisedEquipmentSale = 2; // Allied
        public const int RaceRelationsLevelToAllowShipRepair = 1; // Friendly
        public const int RaceRelationsExperienceScale = 1000; // Experience to get from 0 -> 1 race relations
        public const int StartingRelationsWithHomeRace = 6000; // Experience points. Equivalent to level 2.
        public const int RelationsExpPenaltyScaleColony = 8; // Experience accrues this many times slower for gaining race relations when doing colony missions
        public const int RelationsExpPenaltyScale = 16; // Experience accrues this many times slower for gaining race relations when doing non-colony missions in an alien system

        // Time stuff / global clock
        public static readonly DateTime dtStart = DateTime.ParseExact("2150-01-01 00:00:00", "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
        public static DateTime dtTime = Const.dtStart;
        public static double ElapsedSeconds { get { return (dtTime - dtStart).TotalSeconds; } }

        // Mission level generation settings
        public const int AutomataIterations = 6;
        public const double AutomataCaveFract = 0.43;
        public const double AutomataSurfaceFract = 0.3;
        public const int MinimumCreatureDistanceFromStartLocation = 10;
        public const double CreatureCountExponent = 0.82;  // Scale creature count as level area ^ This
        public const double CreatureCountScale = 100.0;   // The larger this is, the more creatures we put in each level

        // Map viewer settings
        public const double MinAngSize = 0.004; // View settings
        public const float MouseMoveScale = 0.03f;
        public const double MapStarScale = 0.03;
        public const float MinimumMapViewZ = 8.0f;
        public const float MaximumMapViewZ = 50.0f;
        public const float MapViewportAngle = (float)Math.PI / 6.0f;
        public const float InitialMapViewZ = 15.0f;
        public const float MaximumScrollRange = 75.0f;

        // System viewer settings
        public const float SystemViewDist = 20f;
        public const float StarScale = 0.2f;
        public const float PlanetScale = 0.02f;
        public const float MoonScale = 0.7f;  // Multiplied by planet scale
        public const float SystemViewSelectionTolerance = 1.2f;
        public const float MoonGap = 6.5f; // Gap from planet to first displayed moon
        public const float DrawBattleScale = 0.15f / 20000f;  // Scale for ships drawn in the travel battle
        public const float ShipRepairRate = 0.001f;
        public const float ShipShieldRegenRate = 0.001f;

        // Ship viewer settings
        public const float MinimumShipViewZ = 20.0f;
        public const float MaximumShipViewZ = 200.0f;
        public const int SmallRoomWidth = 3;
        public const int SmallRoomHeight = 2;
        public const int MediumRoomWidth = 5;
        public const int MediumRoomHeight = 3;
        public const int LargeRoomWidth = 8;
        public const int LargeRoomHeight = 5;
        public const int StarfieldSize = 2000;
        public const double SalvageScale = 0.5; // Times area of the object
        public const double SalvageRate = 0.25; // Purchase price from vendors as a fraction of the sale price
        public const double ShipEquipmentRepairFract = 0.4; // Ship equipment contributes thiss fraction of its value to repair cost
        public const double ShipRepairCostScale = 0.6;  // Fraction of calculated value required to repair 100%
        public const double HullUpgradeCost = 15.0;  // Divide MaxHull by this for scale factor
        public const double ShipRelativeStrengthScale = 1.8; // If attacking ship is this many times weaker than defending ship then don't even bother...

        // MissionView Settings
        public const float InitialMissionViewZ = 40.0f;
        public const float MinimumMissionViewZ = 8.0f;
        public const float MaximumMissionViewZ = 120.0f;
        public const float TileLayer = 0.0f;
        public const float DoodadLayer = 0.01f;
        public const float EntityLayer = 0.02f;
        public const float GUILayer = 0.05f;

        // MissionView GUI Panel settings
        public const float GUIPanelWidth = 0.16f;
        public const float GUIPanelTop = 0.01f;
        public const float GUIPanelRowHeight = 0.02f;
        public const float GUIPanelGap = 0.015f;
        public const float GUIAlpha = 0.8f;

        // Misc constants
        public const double SpeedOfLight = 299792458.0;  // m/s
        public const double GravitationalConstant = 6.674E-11;  // Scaled by Earth masses
        public const double SecondsPerDay = 3600.0 * 24.0; // Used for working out how long, in seconds, it takes to travel a light year 
        public const double DaysPerYear = 365.2422;
        public const double SecondsPerYear = SecondsPerDay * DaysPerYear; // Used for working out how long, in seconds, it takes to travel a light year 
        public const double LightYear = SpeedOfLight * SecondsPerYear;  // Light year in metres
        public const double AU = 1.5E11;  // Astronomical Unit in metres
        public const double EarthRadius = 6400000.0;
        public const double SunRadius = 7E8;
        public const double SunTemperature = 5780;
        public const double Billion = 1000000000.0;
        public const double Million = 1000000.0;
        public const double Billionth = 0.000000001;
        public const double Millionth = 0.000001;

        // AI Settings
        public const int AITickSpeed = 350;
        public const int FastAITickSpeed = 50;

        // Static constructor
        static Const() {
            // Nothing to see
        }

    }
}
