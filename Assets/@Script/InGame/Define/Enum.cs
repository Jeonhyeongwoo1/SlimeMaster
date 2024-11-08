namespace SlimeMaster.InGame.Enum
{
    public enum GameState
    {
        None = -1,
        Ready,
        Start,
        End
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
    }

    public enum DropableItemType
    {
        Potion,
        Magnet,
        DropBox,
        Bomb,
        Gem
    }

    public enum CreatureType
    {
        None,
        Player = 201000
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
}