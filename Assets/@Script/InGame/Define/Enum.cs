namespace SlimeMaster.InGame.Enum
{
    public enum GameState
    {
        None = -1,
        Ready,
        Start,
        End
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
        PurchaseSupportSkill
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
}