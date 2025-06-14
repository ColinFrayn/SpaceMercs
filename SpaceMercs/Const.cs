using OpenTK.Mathematics;
using System.Globalization;

namespace SpaceMercs {
    public static class Const {
        // Version
        public const string strVersion = "1.0";

        // ---=== DEBUG MODE SETTINGS ===---
        public const float DEBUG_ENCOUNTER_FREQ_MOD = 1;  // Modification factor for encounter frequency. (multiplier, default = 1, <=0 turns off all encounters)
        public const bool  DEBUG_ALL_ENCOUNTERS_INACTIVE = false;  // All ship encounters when travelling are resolved as inactive (repair/salvage)
        public const bool  DEBUG_MORE_BOARDERS = false; // Make it much more likely hostile vessels will board
        public const bool  DEBUG_VIEW_ALL_CIVS = false;  // Set starting systems for all civs = visited
        public const bool  DEBUG_VISIBLE_ALL = false; // Can see the entire map when in a mission
        public const bool  DEBUG_RANDOMISE_VENDORS = true;  // Provide a button to completely regenerate all items/mercs/missions at a colony
        public const bool  DEBUG_SHOW_SELECTED_ENTITY_VIS = false;  // Show visibility of selected entity with dots
        public const bool  DEBUG_MAP_CREATION = false; // Write out to disk intermediate map creation stages
        public const int   DEBUG_WEAPON_SKILL_MOD = 1;  // Weapon skill experience multiplier
        public const int   DEBUG_EXPERIENCE_MOD = 1;  // Experience accrual multiplier
        public const int   DEBUG_ADDITIONAL_STARTING_CASH = 0; // More cash at the start

        // Character settings
        public const int    SpareAttributePoints = 0; // Starting pot of skill points
        public const int    MaximumSkillDeficit = 5; // A skill can't be lowered more than this many points beneath racial average
        public const double MovementCost = 2.0;  // Cost, in stamina points, for moving one square
        public const double MeleeCost = 6.0;  // Cost, in stamina points, for a melee attack
        public const double SearchCost = 10.0; // Cost, in stamina points, for searching the nearby area
        public const double UseItemCost = 10.0; // Cost, in stamina points, for using an item
        public const int    BaseSearchRadius = 5; // Maximum radius of any search for unskilled searcher (increase by 1 per point in perception)
        public const int    PassiveSearchRadius = 2; // Maximum radius of any search for unskilled passive searcher (increase by 1 per point in perception)
        public const double BaseSearchChance = 70.0; // Base chance of spotting a hidden object at distance zero (plus Insight) when actively searching.
        public const double PassiveSearchChance = 30.0; // Base chance of spotting a hidden object at distance zero (plus Insight) when not actively searching.
        public const double BaseSearchReduction = 7.0; // Reduction in chance to spot an object for each metre distant from the searcher
        public const double PassiveSearchReduction = 10.0; // Reduction in chance to spot an object for each metre distant from the searcher
        public const double ActiveSearchBoostPerSkill = 3.0; // Increase in chance to spot a hidden object for each point in Perception.
        public const double PassiveSearchBoostPerSkill = 1.7; // Increase in chance to spot a hidden object for each point in Perception.
        public const double EncumbranceSearchPenalty = 20.0; // Penalty (proportionately applied) for searching when 100% encumbered.
        public const double PerceptionSearchRadiusBoost = 0.5; // Increase in search radius for every point of perception
        public const double MissionDifficultySearchScale = 2.0; // Every extra diff in mission reduces search chance by this.
        public const int    WeaponSkillBase = 25; // Skill required to increase a weapon skill from zero to one. 1-2 requires another twice this, 2-3 requires another 3x etc.
        public const double EquipmentLevelCostExponent = 1.35;  // For each level the weapon quality improves, this is the power law relationship to cost
        public const double EquipmentLevelCostBaseExponent = 1.0;  // For each level the weapon quality improves, additionally multiply cost by this factor 
        public const double EquipmentLevelRarityScale = 0.2;  // Each level of equipment quality makes it this much rarer
        public const double MaxCarryExponent = 1.5; // Affects the maximum amount soldiers can carry before encumbrance affects their stamina regen rate
        public const double MaxCarryScale = 0.9; // Affects the maximum amount soldiers can carry before encumbrance affects their stamina regen rate
        public const double MaxCarryBase = 6.0; // Affects the maximum amount soldiers can carry before encumbrance affects their stamina regen rate
        public const double ArmourCostExponent = 4.5d; // Affects cost scaling for armour based on how much better it is than the base
        public const double ArmourCostMultiplier = 2d; // Affects cost for armour based on how much better the shields are than the base
        public const double ShieldCostMultiplier = 2d; // Affects cost for armour based on how many shields it has
        public const double MassCostMultiplier = 15d; // Affects cost for armour based on how much lighter it is than the base
        public const double MassCostExponent = 0.8d; // Affects cost for armour based on how much lighter it is than the base
        public const double ShieldValueExponent = 1.4d; // Shield value increases nonlinearly
        public const double BonusPhysicalArmourValue = 0.03; // Percent bonus to the cost for each single point of bonus armour for physical damage
        public const double BonusOtherArmourValue = 0.01; // Percent bonus to the cost for each single point of bonus armour for any other damage type
        public const double ModificationCost = 20d; // Cost for a modification for disposable materials
        public const double EncumbranceDefencePenalty = 5d; // Defence penalty for being fully encumbered
        public const double AbilityBonusValue = 10d; // Value bonus for boosting an ability e.g. Strength, Insight
        public const double AbilityBonusExponent = 1.6d; // Value bonus for boosting an ability e.g. Strength, Insight, exponent to make it superlinear
        public const double ArmourSpeedBonusValue = 25d; // Value bonus for boosting an ability e.g. Strength, Insight
        public const double ArmourSpeedBonusExponent = 4d; // Value bonus for boosting an ability e.g. Strength, Insight, exponent to make it superlinear
        public const double PropertyBonusValue = 6d; // Value bonus for boosting a property e.g. Stamina, Health
        public const double PropertyBonusExponent = 1.5d; // Value bonus for boosting a property e.g. Stamina, Health, exponent to make it superlinear
        public const double BaseArmourCost = 6d; // Cost per armour point, base
        public const int    MaxUtilitySkill = 16; // Highest value you can have in any utility skill
        public const double ConstructionChanceScale = 0.9d; // Lower value means that utility skill is less impactful on build success.

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
        public const int    MaxPlanetsPerSystem = 9;
        public const double StarRotation = 25.0 * 24.0 * 3600.0;
        public const double StarRotationSigma = StarRotation / 10.0;
        public const double StarRotationMin = StarRotation / 5.0;
        public const double EarthOrbitalPeriod = 1.0 * 24.0 * 3600.0 * 365.0;
        public const double EarthOrbitalPeriodSigma = EarthOrbitalPeriod / 15.0;
        public const double MoonOrbitalPeriod = 1.0 * 24.0 * 3600.0 * 27.32; // Sidereal month
        public const double MoonOrbitalPeriodSigma = MoonOrbitalPeriod / 15.0;
        public const double DayLength = 24.0 * 3600.0;
        public const double DayLengthSigma = DayLength / 10.0;
        public const int    HomeworldPDensity = 9;
        public const int    HomeworldMinMoons = 2;

        // Planet generation
        public static Vector3 PlanetTypeToCol1(Planet.PlanetType pt) =>
            pt switch {
                Planet.PlanetType.Desert => new Vector3(0.4f, 0.3f, 0),
                Planet.PlanetType.Gas => new Vector3(0.5f, 0.5f, 0.6f),
                Planet.PlanetType.Oceanic => new Vector3(0.0f, 0.0f, 0.4f),
                Planet.PlanetType.Ice => new Vector3(0.0f, 0.0f, 0.5f),
                Planet.PlanetType.Rocky => new Vector3(0.3f, 0.3f, 0.3f),
                Planet.PlanetType.Volcanic => new Vector3(0.3f, 0.0f, 0.0f),
                Planet.PlanetType.Precursor => new Vector3(0.9f, 0.0f, 0.0f),
                _ => new Vector3(0.0f, 0.0f, 0.0f),
            };
        public static Vector3 PlanetTypeToCol2(Planet.PlanetType pt) =>
            pt switch {
                Planet.PlanetType.Desert => new Vector3(0.65f, 0.65f, 0.0f),
                Planet.PlanetType.Gas => new Vector3(0.6f, 0.6f, 0.8f),
                Planet.PlanetType.Oceanic => new Vector3(0.2f, 0.4f, 1.0f),
                Planet.PlanetType.Ice => new Vector3(0.0f, 0.1f, 1.0f),
                Planet.PlanetType.Rocky => new Vector3(0.5f, 0.5f, 0.5f),
                Planet.PlanetType.Volcanic => new Vector3(0.7f, 0.0f, 0.0f),
                Planet.PlanetType.Precursor => new Vector3(0.5f, 0.5f, 0.5f),
                _ => new Vector3(0.0f, 0.0f, 0.0f),
        };
        public static Vector3 PlanetTypeToCol3(Planet.PlanetType pt) =>
            pt switch { 
                Planet.PlanetType.Desert => new Vector3(0.9f, 0.8f, 0.0f),
                Planet.PlanetType.Gas => new Vector3(0.7f, 0.7f, 0.9f),
                Planet.PlanetType.Oceanic => new Vector3(0.0f, 1.0f, 0.0f),
                Planet.PlanetType.Ice => new Vector3(0.0f, 0.2f, 1.0f),
                Planet.PlanetType.Rocky => new Vector3(0.5f, 0.5f, 0.5f),
                Planet.PlanetType.Volcanic => new Vector3(1.0f, 0.0f, 0.0f),
                Planet.PlanetType.Precursor => new Vector3(0.2f, 0.3f, 1.0f),
                _ => new Vector3(0.0f, 0.0f, 0.0f),
        };
        public static Vector3 PlanetTypeToCol4(Planet.PlanetType pt) =>
            pt switch {
                Planet.PlanetType.Desert => new Vector3(1.0f, 1.0f, 0.3f),
                Planet.PlanetType.Gas => new Vector3(0.8f, 0.8f, 1.0f),
                Planet.PlanetType.Oceanic => new Vector3(0.6f, 1.0f, 0.8f),
                Planet.PlanetType.Ice => new Vector3(0.8f, 1.0f, 1.0f),
                Planet.PlanetType.Rocky => new Vector3(0.75f, 0.75f, 0.75f),
                Planet.PlanetType.Volcanic => new Vector3(1.0f, 0.8f, 0.0f),
                Planet.PlanetType.Precursor => new Vector3(1.0f, 1.0f, 0.7f),
                _ => new Vector3(0.0f, 0.0f, 0.0f),
        };
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
        public const int MinStarsPerSector = 12;
        public const int MaxStarsPerSector = 22;
        public const int DefaultStarsPerSector = 14;
        public const double MinStarDistance = 1.0; // Light years

        // Combat settings
        public const double ArmourReductionBase = 0.4; // Base of the armour reduction exponent calculation
        public const double ArmourReductionExponentialScale = 45.0; // Log scale for armour power reduction. The higher this is, the less effective armour is at mitigating damage.
        public const double ArmourReductionSecondExponent = 0.9; // Second exponent to make the armour reduction improve less quickly at larger values.
        public const double CreatureMeleeDamageScale = 0.45;  // The higher this is, the more damage creatures do when attacking in melee without a weapon
        public const double CreatureLevelAttackScale = 0.09;  // %age growth in creature attack each level (over base)
        public const double CreatureLevelAttackStep = 1.0;  // Absolute growth in creature attack each level
        public const double CreatureLevelDefenceScale = 0.09;  // %age growth in creature defence each level (over base)
        public const double CreatureLevelDefenceStep = 0.9;  // Absolute growth in creature defence each level
        public const double CreatureLevelHealthScale = 0.15;  // %age growth in creature health each level (over base)
        public const double CreatureLevelHealthStep = 1.5;  // Absolute growth in creature health each level
        public const double CreatureLevelShieldsScale = 0.1;  // %age growth in creature shields each level (over base)
        public const double CreatureLevelArmourScale = 0.1;  // %age growth in creature armour each level (over base)
        public const double CreatureLevelArmourStep = 1.0;  // Absolute growth in creature armour each level
        public const double CreatureLevelStaminaStep = 1.0;  // Absolute growth in creature stamina each level
        public const double HitBias = 4.0; // Bias towards hitting when attacking
        public const double SoldierHitBias = 0.0; // Bias towards a player-controlled soldier hitting when attacking
        public const double RandomHitScale = 10.0; // Add extra randomness to help cancel out "certain hit" or "certain miss"
        public const double AttackScale = 0.8; // Bias towards attack score
        public const double DefenceScale = 0.8; // Bias towards defence score
        public const double GuaranteedHitScale = 0.5;  // The higher this is, the higher the weighting for absolute att-def, leading to more predictable hits/misses
        public const double EncumbranceHitPenalty = 4.0; // When fully encumbered, reduce hit rolls by this much (or proportionately).
        public const double HitSizePowerLaw = 2.0;  // Increase chance to hit for larger creatures
        public const double PartialCoverDefenceBonus = 2.5; // When hiding by a wall from perspective of the (ranged) attacker, increase defence by this much  ##UNUSED##
        public const double BaseDetectionRange = 7.0; // Range in squares at which a creature of same level can spot you, if you're unencumbered and with default agility
        public const int    CreatureAlertWarningDistance = 4;  // Range to which any alerted creature can trigger other creatures that they can see to also be alert
        public const double FireWeaponExtraDetectionRange = 4.0; // If a soldier fires his weapon then alert all entities no more than this distance outside his detection range
        public const double CreatureExperienceScale = 1d;  // Scale creature experience value by this amount
        public const double TrapDamageScale = 2.0;  // Increase damage done by traps
        public const double CreatureAttackDamageScale = 0.085;  // Modifier applied to damage done by Creatures per point of Attack.
        public const double CreatureAttackDamageBaseMod = 0.8; // Base scale factor for creature attack damage.
        public const double SoldierAttackDamageScale = 0.1;  // Modifier applied to damage done by player-controlled Soldiers per point of Attack.
        public const double TurnLength = 10.0; // Length of one combat turn in seconds
        public const double SurpriseHitMod = 5.0; // Hit bonus if the target is not alert to your presence
        public const float  ShotDurationScale = 25.0f; // Time taken for shots to get from source to target
        public const float  ShotSizeScale = 250.0f; // Size of a shot line (larger = smaller)
        public const float  ShotScatterScale = 0.15f; // Amount of visual scatter on shot lines
        public const double HeavyWeaponMeleeDefencePenalty = 8d; // Defence penalty when wielding a heavy weapon and being attacked in melee
        public const double MeleeEncumbranceAttackPenalty = 8d; // Penalty to melee attack when fully encumbered.
        public const double MeleeEncumbranceDefencePenalty = 8d; // Penalty to defence when fully encumbered and attacked in melee.
        public const double DiagonalMeleePenalty = -5d; // Penalty for attacking diagonally with a short melee weapon
        public const bool   EnableCreatureMeleeDefenceBonus = false; // If true then melee creatures get a defence bonus agaisnt melee attackers
        public const bool   EnableSoldierMeleeDefenceBonus = false; // If true then melee soldiers get a defence bonus agaisnt melee attackers

        // Miscellaneous
        public const double InitialCash = 50.0;
        public const int    MaxItemLevel = 5;
        public const int    SoldierLevelExperience = 1000; // Experience for level 1->2
        public const double SoldierLevelExponent = 1.6;  // Each level gets this many times further apart
        public const double SoldierLevelScale = 4.0; // Experience scale
        public const int    ItemIDBase = 2000000; // ID of first ItemType
        public const int    NextThingIDBase = 3000000; // ID of first NextThing (whatever that might be). Give sufficient space between them
        public const double BaseEncounterScarcity = 1500.0;  // Base chance for an encounter when travelling (higher = less frequent)
        public const double InterstellarTravelSafetyBonus = 15.0; // Less likely to get intercepted in interstellar space
        public const double BaseInterceptionChance = 40.0; // Base scaling for encountering something during a journey. The higher the more often and the more dangerous.
        public const double EncounterLevelScalingInnerRadius = 5.0; // Size of the inner scaling region, in ly.
        public const double EncounterLevelScalingDistanceInner = 1.0; // Within the inner scaling region, this is the base difficulty increase scale length.
        public const double EncounterLevelScalingExponentInner = 1.0; // 1 means encounter difficulty scales linearly. 0 means no scaling. 0.5 = sqrt, etc.
        public const double EncounterLevelScalingDistanceOuter = 3.3; // Outside of the inner scaling region, this is the base difficulty increase scale length.
        public const double EncounterLevelScalingExponentOuter = 0.85; // 1 means encounter difficulty scales linearly. 0 means no scaling. 0.5 = sqrt, etc.
        public const double EncounterFreqScale = 0.15; // The lower this is (>0.0) the less frequently we encounter anything when travelling (active or passive).
        public const double ShipBountyScale = 0.15;  // Scale for calculating bounty of enemy ships defeated
        public const double HyperspaceGateTimeFactor = 40.0; // Travelling by Hyperspace gate is effectively this many times faster than light speed.
        public const double HyperspaceCostScale = 15.0; // Divide the cost of the hyperspace travel by this factor
        public const double HyperspaceCostDistanceExponent = 1.1; // Exponent for distance in Ly when calculating cost. 1 = linear.
        public const double HyperspaceCostHullExponent = 1.2; // Exponent for hull size when calculating cost. 1 = linear.
        public const int    InitialCivilisationSize = 20; // Total population of all civs at the start
        public const double BasicTradeRouteLength = 4.0; // Maximum length of a trade route in light years for a basic colony
        public const double MaxTradeRouteLength = 7.0; // Maximum length of a trade route in light years for a large colony
        public const double DailyResearchProb = 0.0015; // Chance of successfully researching a technology, per day, for a nonHuman race.
        public const double UpgradeSelfCostMod = 0.75; // When upgrading stuff yourself, pay this fraction of the total price estimate.
        public const int    SkillBoostPerAIModule = 2; // Each AI Module boosts utility skills by this much.
        public const double SecondaryEnemyXPBoost = 1.25; // This much more XP if there is a secondary enemy
        public const double MissionExperienceScale = 0.4d; // Scale for experience gained for finishing a mission
        public const double CashRelationsFactor = 0.25; // Exp per credit donated
        public const int    SpaceHulkCoreExpScale = 125; // Exp per level for a space hulk core
        public const int    PrecursorCoreExpScale = 175; // Exp per level for a precursor core

        // Colony stuff
        public const int    MaxColonyMercenaries = 16;  // In a colony
        public const int    MaxColonyMissions = 16;  // In a colony
        public const double TradeRouteColonyGrowthRate = 0.75; // Colonies grow more quickly when system has trade route(s) (modify delay by this amount)
        public const double UnconnectedColonyCostMod = 1.5; // Everything is more expensive (and valuable) in distant systems without trade routes
        public const int    LongEnoughGapToResetColonyInventory = 365; // Number of days after which we just throw away the old inventory
        public const double MercenaryCostScale = 6.0;  // Base price scale of a mercenary, not including kit
        public const double MercenaryCostBase = 1.18;  // Base for exponential price calculation for a mercenary
        public const double MercenaryCostExponent = 1.8;  // Exponent scale for exponential price calculation for a mercenary
        public const double MercenaryKitValueScale = 1.0; // Mercenary kit is discounted by this amount
        public const int    MerchantStockResetDuration = 50;  // When completely resettign a merchant's store, how many days worth of incoming stock do you generate?
        public const double UpgradeCostModifier = 1.2; // To upgrade an item by one level
        public const double SellDiscount = 0.5; // Get this fraction back when selling an item
        public const double ColonySeedRate = 5.0; // Affects the speed at which new colonies are created in populated systems.
        public const double ColonySeedTarget = 200.0; // Target seeding point at which level the colony may seed
        public const double GrowthExponent = 1.8; // Affects how time taken for a colony to grow scales with size
        public const double GrowthScale = 2.5; // Affects how time taken for a colony to grow scales with size
        public const double GrowthTempBase = 1.1; // Affects how time taken for a colony to grow scales with temperature diff.
        public const double GrowthTempScale = 10; // Affects how time taken for a colony to grow scales with temperature diff.
        public const double GrowthTempOffset = 15; // The range of temperatures away from ideal within which colony growth is not affected
        public const double MissionCashScale = 0.1d; // Scale cash reward for colony missions by this amount.
        public const double ColonyMissionGrowthBonus = 30d; // When completing an average colony mission, reduce the time to next growth of the current colony by this many days. Scale this by difficulty/length.

        // Race relations
        public const int InitialColonyCount = 3;   // When starting up, how many colonies to add to a system
        public const int HomeSysColonyCount = 3;   // Extra colonies for home system
        public const int RaceRelationsLevelToAllowSpecialisedEquipmentSale = 2; // Allied
        public const int RaceRelationsLevelToAllowShipRepair = 1; // Friendly
        public const int RaceRelationsExperienceScale = 1000; // Experience to get from 0 -> 1 race relations is 2x this number. Scales up after this by Lev*(Lev+1) (or Lev*(1-Lev) for negative)
        public const int StartingRelationsWithHomeRace = 6000; // Initial HumanRace experience points. Equivalent to level 2.
        public const int RelationsExpPenaltyScaleColony = 4; // Race relation experience accrual scale (reduction factor) for doing colony missions.
        public const int RelationsExpPenaltyScale = 6; // Race relation experience accrual scale (reduction factor) for doing non-colony (scanner) missions but in an owned system.
        public const int FoundColonyRelationsBonus = 500; // Race relation bonus for founding a colony
        public const int FoundColonyInContestedSystemRelationsPenalty = -4000; // Race relation penalty for founding a colony in a system they own
        public const int FoundColonyInHomeSectorRelationsPenalty = -1500; // Race relation penalty for founding a colony in a sector they own

        // Time stuff / global clock
        public static readonly DateTime StartingDate = DateTime.ParseExact("2150-01-01 00:00:00", "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);

        // Mission level generation settings
        public const int AutomataIterations = 6;
        public const double AutomataCaveFract = 0.43;
        public const double AutomataSurfaceFract = 0.35;
        public const int MinimumCreatureDistanceFromStartLocation = 10;
        public const double CreatureCountExponent = 0.86;  // Scale creature count as NumFloorTiles ^ ThisValue
        public const double CreatureFrequencyScale = 180;   // The larger this is, the fewer creatures we put in each level.
        public const int LegendaryItemLevelDiff = 5; // Minimum level diff to have a legendary item of research level 0;

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
        public const double ShipRepairCostScale = 0.6;  // Fraction of calculated value required to repair 100%
        public const double HullUpgradeCost = 1.0;  // Scale factor for ship hull/armour upgrade cost.
        public const double ShipRelativeStrengthScale = 2.0; // If attacking ship is this many times weaker than defending ship then don't even bother...

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
