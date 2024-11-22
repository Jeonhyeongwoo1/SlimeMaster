namespace SlimeMaster.Enum
{
    public enum GameState
    {
        None = -1,
        Ready,
        Start,
        DeadPlayer
    }
    
    public enum CreatureStateType
    {
        None,
        Idle,
        Move,
        Skill,
        Dead
    }

    public enum GameEventType
    {
        None = -1,
        //InGame
        DeadPlayer,
        ResurrectionPlayer,
        GameOver,
        SpawnMonster = 100,
        DeadMonster,
        LevelUp,
        UpgradeOrAddNewSkill,
        TakeDamageEliteOrBossMonster,
        ActivateDropItem,
        SpawnedBoss,
        EndWave,
        LearnSkill,
        PurchaseSupportSkill,
        //OutGame
        GetReward,
        MoveToTap,
        ShowEquipmentInfoPopup,
        OnUpdatedEquipment,
        ShowGachaListPopup,
        ShowGachaResultPopup,
        ShowMergePopup,
        ShowMergeResultPopup,
        ShowOutGameContentPopup
    }

    public enum DropableItemType
    {
        Potion,
        Magnet,
        DropBox,
        Bomb,
        Gem,
        Soul
    }

    public enum CreatureType
    {
        None,
        Player = 201000,
        Monster
    }

    public enum MonsterType
    {
        None,
        Normal,
        Elete,
        Boss
    }

    public enum SkillType
    {
        None = 0,
        EnergyBolt = 10001,       //100001 ~ 100005 
        IcicleArrow = 10011,          //100011 ~ 100015 
        PoisonField = 10021,      //100021 ~ 100025 
        EletronicField = 10031,   //100031 ~ 100035 
        Meteor = 10041,           //100041 ~ 100045 
        FrozenHeart = 10051,      //100051 ~ 100055 
        WindCutter = 10061,       //100061 ~ 100065 
        EgoSword = 10071,         //100071 ~ 100075 
        ChainLightning = 10081,
        Shuriken = 10091,
        ArrowShot = 10101,
        SavageSmash = 10111,
        PhotonStrike = 10121,
        StormBlade = 10131,
        MonsterRangedAttackSkill = 20091,
        BossSkill = 100001,
        BasicAttack = 100101,
        Move = 100201,
        Charging = 100301,
        Dash = 100401,
        SpinShot = 100501,
        CircleShot = 100601,
        ComboShot = 100701,
    }

    public enum GemType
    {
        None,
        SmallGem,
        GreenGem,
        BlueGem,
        YellowGem
    }

    public enum SceneType
    {
        TitleScene,
        LobbyScene,
        GameScene,
    }
    
    public enum SupportSkillName
    {
        Critical,
        MaxHpBonus,
        ExpBonus,
        SoulBonus,
        DamageReduction,
        AtkBonusRate,
        MoveBonusRate,
        Healing, // 체력 회복 
        HealBonusRate,//회복량 증가
        HpRegen,
        CriticalDamage,
        MagneticRange,
        Resurrection,
        LevelupMoveSpeed,
        LevelupReduction,
        LevelupAtk,
        LevelupCri,
        LevelupCriDmg,
        MonsterKillAtk,
        MonsterKillMaxHP,
        MonsterKillReduction,
        EliteKillExp,
        EliteKillSoul,
        EnergyBolt,
        IcicleArrow,
        PoisonField,
        EletronicField,
        Meteor,
        FrozenHeart,
        WindCutter,
        EgoSword,
        ChainLightning,
        Shuriken,
        ArrowShot,
        SavageSmash,
        PhotonStrike,
        StormBlade,
    }

    public enum SupportSkillGrade
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legend
    }

    public enum SupportSkillType
    {
        General,
        Passive,
        LevelUp,
        MonsterKill,
        EliteKill,
        Special
    }
    
    public enum MaterialType
    {
        None,
        Gold = 50001,
        Dia,
        Stamina,
        Exp,
        WeaponScroll = 50101,
        GlovesScroll,
        RingScroll,
        BeltScroll,
        ArmorScroll,
        BootsScroll,
        BronzeKey = 50201,
        SilverKey,
        GoldKey,
        RandomScroll = 50301,
        AllRandomEquipmentBox,
        CommonEquipmentBox,
        UncommonEquipmentBox,
        RareEquipmentBox,
        EpicEquipmentBox,
        LegendaryEquipmentBox,
        WeaponEnchantStone = 50401,
        GlovesEnchantStone,
        RingEnchantStone,
        BeltEnchantStone,
        ArmorEnchantStone,
        BootsEnchantStone,
    }

    public enum MaterialGrade
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Epic1,
        Epic2,
        Legendary,
        Legendary1,
        Legendary2,
        Legendary3,
    }
    
    public enum ToggleType
    {
        None,
        BattleToggle,
        EquipmentToggle,
        ShopToggle
    }

    public enum ServerErrorCode
    {
        Success,
        FailedFirebaseError,
        FailedGetUserData,
        NotEnoughStamina,
        FailedGetStage,
        NotEnoughGold,
        NotEnoughMaterialAmount,
        FailedGetEquipment,
        FailedGetItemData,
        NotEnoughShopCostItemValue,
        NotEnoughTime,
        AlreadyClaimed,
    }

    public enum WaveType
    {
        
    }
    
    public enum WaveClearType
    {
        FirstWaveClear,
        SecondWaveClear,
        ThirdWaveClear
    }
    
    public enum GachaRarity
    {
        Normal,
        Special,
    }
    
    public enum MergeEquipmentType
    {
        None,
        ItemCode,
        Grade,
    }
    
    public enum EquipmentType
    {
        Weapon,
        Gloves,
        Ring,
        Belt,
        Armor,
        Boots,
    }

    public enum EquipmentGrade
    {
        None,
        Common,
        Uncommon,
        Rare,
        Epic,
        Epic1,
        Epic2,
        Legendary,
        Legendary1,
        Legendary2,
        Legendary3,
        Myth,
        Myth1,
        Myth2,
        Myth3
    }

    public enum EquipmentSortType
    {
        Level,
        Grade,
    }

    public enum ShopType
    {
        CommonItem,
        EquipmentItem,
        Advance_EquipmentItem,
        Normal_EquipmentItem,
        Gold
    }

    public enum ShopItemType
    {
        Ad,
        SilverKeyItem,
        GoldKeyItem,
        Normal,
        EquipmentItem_one,
        EquipmentItem_ten
    }
    
    public enum GachaType
    {
        None,
        CommonGacha,
        AdvancedGacha,
        PickupGacha,
    }

    public enum EquipAbilityStatType
    {
        Grade,
        Level,
        ATK,
        HP
    }

    public enum OutGameContentButtonType
    {
        Setting,
        Checkout,
        Mission,
        Achievement
    }
}