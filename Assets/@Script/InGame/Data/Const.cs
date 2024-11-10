using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.InGame.Data
{
    public class Const
    {
        public const int MAX_SKILL_Level = 6;
        public const int MAX_SKILL_COUNT = 6;
        public const int SKILL_CARD_ITEM_COUNT = 3;
        
        #region 보석 경험치 획득량
        public const int SMALL_EXP_AMOUNT = 1;
        public const int GREEN_EXP_AMOUNT = 2;
        public const int BLUE_EXP_AMOUNT = 5;
        public const int YELLOW_EXP_AMOUNT = 10;
        #endregion


        #region PoolId

        public const string ExpGem = "ExpGem";
        public const string MeteorShadow = "MeteorShadow";
        public const string BossSmashHitEffect = "BossSmashHitEffect";

        #endregion
        
        #region 데이터아이디
        public static int ID_GOLD = 50001;
        public static int ID_DIA = 50002;
        public static int ID_STAMINA = 50003;
        public static int ID_BRONZE_KEY = 50201;
        public static int ID_SILVER_KEY = 50202;
        public static int ID_GOLD_KEY = 50203;
        public static int ID_RANDOM_SCROLL = 50301;
        public static int ID_POTION = 60001;
        public static int ID_MAGNET = 60004;
        public static int ID_BOMB = 60008;

        public static int ID_WEAPON_SCROLL = 50101;
        public static int ID_GLOVES_SCROLL = 50102;
        public static int ID_RING_SCROLL = 50103;
        public static int ID_BELT_SCROLL = 50104;
        public static int ID_ARMOR_SCROLL = 50105;
        public static int ID_BOOTS_SCROLL = 50106;

        public static string GOLD_SPRITE_NAME = "Gold_Icon";
        #endregion
        public static int MAX_STAMINA = 50;
        public static int GAME_PER_STAMINA = 3;
        
        //데이터 아이디에 따른 포션 회복량
        public static readonly Dictionary<int, float> DicPotionAmount = new Dictionary<int, float>
        {
            { 60001, 0.3f }, // 에픽 등급 랜덤 장비 상자 
            { 60002, 0.5f }, // 골드
            { 60003, 1 }, // 랜덤 스크롤
        };
    }
}