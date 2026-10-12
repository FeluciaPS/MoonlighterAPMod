namespace MoonlighterAPMod
{
    internal class MoonlighterAPConstants
    {
        internal class WeaponType
        {
            internal string name;
            internal string[] powerPath;
            internal string[] elementalPath;
        }

        internal static readonly string[] STARTING_WEAPON_NAMES =
        {
            "Broom Spear",
            "Training Short Sword",
            "Training Big Sword",
            "Training Spear",
            "Training Gloves",
            "Training Bow"
        };

        internal static readonly WeaponType SHORT_SWORD = new()
        {
            name = "Short Sword",
            powerPath = [
                "Soldier",
                "Knight",
                "Commander",
                "King"
            ],
            elementalPath = [
                "Rusty",
                "Venom",
                "Reborn",
                "Vampire"
            ]
        };

        internal static readonly WeaponType BIG_SWORD = new()
        {
            name = "Big Sword",
            powerPath = [
                "Buster",
                "Wild",
                "Vulcan",
                "Fusion"
            ],
            elementalPath = [
                "Rock",
                "Toxic",
                "Blaze",
                "Storm"
            ]
        };

        internal static readonly WeaponType SPEAR = new()
        {
            name = "Spear",
            powerPath = [
                "Warrior",
                "Wood's",
                "Monkey",
                "Fighter"
            ],
            elementalPath = [
                "Golem Drill",
                "Venom Sting",
                "Hell",
                "Lightning Rod"
            ]
        };

        internal static readonly WeaponType GLOVES = new()
        {
            name = "Gloves",
            powerPath = [
                "Fighter",
                "Forest Spirit",
                "Captain",
                "Star Platinum"
            ],
            elementalPath = [
                "Rough",
                "Venom Twins",
                "Flame",
                "Thunder"
            ]
        };

        internal static readonly WeaponType BOW = new()
        {
            name = "Bow",
            powerPath = [
                "Hunter",
                "Natural",
                "Soldier",
                "Exeter"
            ],
            elementalPath = [
                "Catapult",
                "Poison",
                "Flamethrower",
                "Lightning"
            ]
        };

        internal static readonly WeaponType[] WEAPON_TYPES = [
            SHORT_SWORD,
            BIG_SWORD,
            SPEAR,
            GLOVES,
            BOW
        ];

        internal static WeaponType GetWeaponObject(string name)
        {
            return WEAPON_TYPES.FirstOrDefault(obj => obj.name == name);
        }

        // TODO: We can do better I think
        internal static readonly string[] ALL_EQUIPMENT_NAMES = [
            "Training Short Sword",
            "Soldier Short Sword",
            "Rusty Short Sword",
            "Knight Short Sword",
            "Venom Short Sword",
            "Commander Short Sword",
            "Reborn Short Sword",
            "King Short Sword",
            "Vampire Short Sword",
            "Training Big Sword",
            "Buster Big Sword",
            "Rock Big Sword",
            "Wild Big Sword",
            "Toxic Big Sword",
            "Vulcan Big Sword",
            "Blaze Big Sword",
            "Fusion Big Sword",
            "Storm Big Sword",
            "Broom Spear",
            "Training Spear",
            "Warrior Spear",
            "Golem Drill Spear",
            "Wood's Spear",
            "Venom Sting Spear",
            "Monkey Spear",
            "Hell Spear",
            "Fighter Spear",
            "Lightning Rod Spear",
            "Training Gloves",
            "Fighter Gloves",
            "Rough Gloves",
            "Forest Spirit Gloves",
            "Venom Twins Gloves",
            "Captain Gloves",
            "Flame Gloves",
            "Star Platinum Gloves",
            "Thunder Gloves",
            "Training Bow",
            "Hunter Bow",
            "Catapult Bow",
            "Natural Bow",
            "Poison Bow",
            "Soldier Bow",
            "Flamethrower Bow",
            "Exeter Bow",
            "Lightning Bow",
            "Fabric Bandana",
            "Fabric Bandana II",
            "Fabric Bandana III",
            "Fabric Bandana IV",
            "Iron Helmet",
            "Iron Helmet II",
            "Iron Helmet III",
            "Iron Helmet IV",
            "Steel Helmet",
            "Steel Helmet II",
            "Steel Helmet III",
            "Steel Helmet IV",
            "Fabric Chestplate",
            "Fabric Chestplate II",
            "Fabric Chestplate III",
            "Fabric Chestplate IV",
            "Iron Chestplate",
            "Iron Chestplate II",
            "Iron Chestplate III",
            "Iron Chestplate IV",
            "Steel Chesplate",
            "Steel Chesplate II",
            "Steel Chesplate III",
            "Steel Chesplate IV",
            "Fabric Boots",
            "Fabric Boots II",
            "Fabric Boots III",
            "Fabric Boots IV",
            "Iron Boots",
            "Iron Boots II",
            "Iron Boots III",
            "Iron Boots IV",
            "Steel Boots",
            "Steel Boots II",
            "Steel Boots III",
            "Steel Boots IV"
        ];

        internal readonly static string[] TRAP_NAMES = [];

        internal readonly static long[] TUTORIAL_LOCATIONS = [2, 3];

        internal readonly static string[][] HAWKER_ITEMS = [
            [
                "GolemWallFountain",
                "GolemWallCloth",
                "GolemTableBooks",
                "GolemTableCrystal",
            ],
            [
                "ForestWallFlowerpot",
                "ForestWallPinkFlowerpot",
                "ForestTableFruitVessel",
                "ForestTableBossFlower",
            ],
            [
                "DesertWallCloth",
                "DesertWallLavaFountain",
                "DesertTableLavaSand",
                "DesertTableBossSnakeHead",
            ],
            [
                "TeslaWallShelve",
                "TeslaWallCoils",
                "TeslaTableLightBulb",
                "TeslaTableBossBall"
            ]
        ];
    }
}
