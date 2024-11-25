using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.Data
{
    public class Const
    {
        public const int PLAYER_DATA_ID = 201000;
        public const int GAME_START_STAMINA_COUNT = 3;
        public const int WAVE_COUNT = 3;
        public const int MAX_SKILL_Level = 6;
        public const int MAX_SKILL_COUNT = 6;
        public const int SKILL_CARD_ITEM_COUNT = 3;
        
        #region 보석 경험치 획득량
        public const int SMALL_EXP_AMOUNT = 1;
        public const int GREEN_EXP_AMOUNT = 2;
        public const int BLUE_EXP_AMOUNT = 5;
        public const int YELLOW_EXP_AMOUNT = 10;
        #endregion

        #region 가챠 확률
        public static readonly float[] SUPPORTSKILL_GRADE_PROB = new float[]
        {
            //0.04f,   // Common 확률
            //0.04f,   // Uncommon 확률
            //0.01f,   // Rare 확률
            //0.5f,  // Epic 확률
            //0.45f,  // Legend 확률

            0.4f,   // Common 확률
            0.4f,   // Uncommon 확률
            0.1f,   // Rare 확률
            0.07f,  // Epic 확률
            0.03f,  // Legend 확률

        };
        
        public static readonly float[] COMMON_GACHA_GRADE_PROB = new float[]
        {
            0,
            0.62f,   // Common 확률
            0.18f,   // Uncommon 확률
            0.15f,   // Rare 확률
            0.05f,  // Epic 확률
        };

        public static readonly float[] ADVENCED_GACHA_GRADE_PROB = new float[]
        {
            0,
            0.55f,   // Common 확률
            0.20f,   // Uncommon 확률
            0.15f,   // Rare 확률
            0.10f,  // Epic 확률
        };

        public static readonly float[] PICKUP_GACHA_GRADE_PROB = new float[]
        {
            0,
            0.55f,   // Common 확률
            0.20f,   // Uncommon 확률
            0.15f,   // Rare 확률
            0.10f,  // Epic 확률
        };
        
        #endregion

        #region PoolId

        public const string ExpGem = "ExpGem";
        public const string Soul = "Soul";
        public const string MeteorShadow = "MeteorShadow";
        public const string BossSmashHitEffect = "BossSmashHitEffect";
        public const string Revival = "Revival";

        #endregion
        
        public static int STAGE_SOULCOUNT = 10;
        public static float STAGE_SOULDROP_RATE = 1f;//0.05f;
        public static int SUPPORT_ITEM_USEABLECOUNT = 4;
        public static int CHANGE_SUPPORT_SKILL_AMOUNT = 80;
        public static int DEFAULT_MagneticRange = 3;

        public static int MONSTER_KILL_BONUS_COUNT = 500;
        
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
        public static string HourPerName = "시간당";
        public static int MIN_OFFLINE_REWARD_MINUTE = 10;
        public static int FAST_REWARD_USE_STAMINA_COUNT = 15;
        public static int FAST_REWARD_GOLD_MULITPILIER = 5;
        
        //데이터 아이디에 따른 포션 회복량
        public static readonly Dictionary<int, float> DicPotionAmount = new Dictionary<int, float>
        {
            { 60001, 0.3f }, // 에픽 등급 랜덤 장비 상자 
            { 60002, 0.5f }, // 골드
            { 60003, 1 }, // 랜덤 스크롤
        };
        
        
        public static class EquipmentUIColors
        {
            #region 장비 이름 색상
            public static readonly Color CommonNameColor = Utils.HexToColor("A2A2A2");
            public static readonly Color UncommonNameColor = Utils.HexToColor("57FF0B");
            public static readonly Color RareNameColor = Utils.HexToColor("2471E0");
            public static readonly Color EpicNameColor = Utils.HexToColor("9F37F2");
            public static readonly Color LegendaryNameColor = Utils.HexToColor("F67B09");
            public static readonly Color MythNameColor = Utils.HexToColor("F1331A");
            #endregion
            #region 테두리 색상
            public static readonly Color Common = Utils.HexToColor("AC9B83");
            public static readonly Color Uncommon = Utils.HexToColor("73EC4E");
            public static readonly Color Rare = Utils.HexToColor("0F84FF");
            public static readonly Color Epic = Utils.HexToColor("B740EA");
            public static readonly Color Legendary = Utils.HexToColor("F19B02");
            public static readonly Color Myth = Utils.HexToColor("FC2302");

            public static Color GetMaterialGradeColor(MaterialGrade materialGrade)
            {
                switch (materialGrade)
                {
                    case MaterialGrade.Common:
                        return Common;
                    case MaterialGrade.Uncommon:
                        return Uncommon;
                    case MaterialGrade.Rare:
                        return Rare;
                    case MaterialGrade.Epic:
                    case MaterialGrade.Epic1:
                    case MaterialGrade.Epic2:
                        return Epic;
                    case MaterialGrade.Legendary:
                    case MaterialGrade.Legendary1:
                    case MaterialGrade.Legendary2:
                    case MaterialGrade.Legendary3:
                        return Legendary;
                }

                Debug.LogError($"Failed {nameof(GetEquipmentGradeColor)} / grade {materialGrade}");
                return Color.white;
            }
            
            public static Color GetEquipmentGradeColor(EquipmentGrade equipmentGrade)
            {
                switch (equipmentGrade)
                {
                    case EquipmentGrade.Common:
                        return Common;
                    case EquipmentGrade.Uncommon:
                        return Uncommon;
                    case EquipmentGrade.Rare:
                        return Rare;
                    case EquipmentGrade.Epic:
                    case EquipmentGrade.Epic1:
                    case EquipmentGrade.Epic2:
                        return Epic;
                    case EquipmentGrade.Legendary:
                    case EquipmentGrade.Legendary1:
                    case EquipmentGrade.Legendary2:
                    case EquipmentGrade.Legendary3:
                        return Legendary;
                    case EquipmentGrade.Myth:
                    case EquipmentGrade.Myth1:
                    case EquipmentGrade.Myth2:
                    case EquipmentGrade.Myth3:
                        return Myth;
                }

                Debug.LogError($"Failed {nameof(GetEquipmentGradeColor)} / grade {equipmentGrade}");
                return Color.white;
            }
            #endregion
            #region 배경색상
            public static readonly Color EpicBg = Utils.HexToColor("D094FF");
            public static readonly Color LegendaryBg = Utils.HexToColor("F8BE56");
            public static readonly Color MythBg = Utils.HexToColor("FF7F6E");
            #endregion
        }
        
        #region 유저 첫 기본 장비 아이디
        public static readonly string DefaultWeaponId = "N00301";
        public static readonly string DefaultGlovesId = "N10101";
        public static readonly string DefaultRingId = "N20101";
        public static readonly string DefaultBeltId = "N30101";
        public static readonly string DefaultArmorId = "N40101";
        public static readonly string DefaultBootsId = "N50101";
        #endregion
    }
}