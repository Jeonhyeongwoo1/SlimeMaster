using System;
using UnityEngine.Serialization;

namespace SlimeMaster.InGame.Data
{
    [Serializable]
    public class GameContinueData
    {

        public PlayerStat playerStat;

        public GameContinueData()
        {
            playerStat = new PlayerStat();
        }
    }

    [Serializable]
    public class PlayerStat
    {
        public float criRate;
        public float maxHPBonus = 1;
        public float expBonusRate = 1;
        public float soulBonusRate = 1;
        public float damageReduction = 0;
        public float defRate;
        public float attackBonusRate = 1;
        public float moveSpeedRate = 1;
        public float healing;
        public float healBonusRate = 1;
        public float hpRegen;
        public float criticalDamage = 1.5f;
        public float magneticRange = 1f;
        public float resurrection;

        public float maxHP;
        public float HP;
    }
}