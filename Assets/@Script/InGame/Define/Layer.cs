using UnityEngine;

namespace SlimeMaster.InGame.Enum
{
    public static class Layer
    {
        public static readonly int BossAndMonster = LayerMask.GetMask("Monster", "Boss");
        public static readonly int MonsterLayer = LayerMask.GetMask("Monster");
        public static readonly int BossLayer = LayerMask.GetMask("Boss");
    }

    public static class Tag
    {
        public static readonly string Monster = "Monster";
        public static readonly string Player = "Player";
    }
}