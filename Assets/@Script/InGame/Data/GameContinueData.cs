using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.InGame.Skill;
using UnityEngine.Serialization;

namespace SlimeMaster.InGame.Data
{
    [Serializable]
    public class GameContinueData
    {
        public bool IsContinue => killCount > 0;
        
        public PlayerStat playerStat;
        public int soulAmount;
        public int stageIndex;
        public int waveIndex;

        public int playerExp;
        public int playerLevel;
        public int killCount;

        public List<SkillData> skillList = new();
        public List<SupportSkillData> supportSkillDataList = new();
        
        public GameContinueData()
        {
            Reset();
        }

        public void Reset()
        {
            playerStat = new PlayerStat();
            soulAmount = 0;
            waveIndex = 0;
            stageIndex = 1;
            playerLevel = 1;
            playerExp = 0;
            killCount = 0;
            skillList.Clear();
            supportSkillDataList.Clear();
        }
    }

    [Serializable]
    public class PlayerStat
    {
        public int level = 1;
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