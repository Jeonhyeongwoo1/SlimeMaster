using System;
using System.Collections.Generic;
using System.Threading;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Managers;
using SlimeMaster.Presenter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SlimeMaster.Common
{
    public static class Utils
    {
        public static EquipmentGrade GetRandomEquipmentGrade(float[] prob)
        {
            float value = Random.value;
            if (value < prob[(int)EquipmentGrade.Epic])
            {
                return EquipmentGrade.Epic;
            }

            if (value < prob[(int)EquipmentGrade.Rare])
            {
                return EquipmentGrade.Rare;
            }

            if (value < prob[(int)EquipmentGrade.Uncommon])
            {
                return EquipmentGrade.Uncommon;
            }

            return EquipmentGrade.Common;
        }
        
        public static T[] GetChildComponent<T>(Transform parent) where T  : Object
        {
            T[] childs = parent.GetComponentsInChildren<T>(true);
            if (childs.Length == 0)
            {
                return null;
            }

            T[] c = new T[childs.Length];
            for (var i = 0; i < childs.Length; i++)
            {
                if (parent == childs[i])
                {
                    continue;
                }

                c[i] = childs[i];
            }

            return c;
        }
        
        public static void SafeAddButtonListener(this Button button, UnityAction listener)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(listener);
        }
        
        public static List<T> Shuffle<T>(this List<T> shuffleList)
        {
            int n = shuffleList.Count;
            while (n > 1)
            {
                n--;
                int random = Random.Range(0, n + 1);
                (shuffleList[random], shuffleList[n]) = (shuffleList[n], shuffleList[random]);
            }
            
            return shuffleList;
        }
        
        public static void SafeCancelCancellationTokenSource(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }
        
        public static Transform[] GetChilds(this Transform tr)
        {
            Transform[] children = new Transform[tr.transform.childCount];
            for (int i = 0; i < tr.transform.childCount; i++)
            {
                children[i] = tr.GetChild(i);
            }

            return children;
        }

        public static bool TryGetComponentInParent<T>(GameObject gameObject, out T t) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out t))
            {
                return true;
            }

            var component = gameObject.GetComponentInParent<T>();
            if (component != null)
            {
                t = component;
                return true;
            }

            return false;
        }

        public static T AddOrGetComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }
        
        public static Vector3 GetPositionInDonut(Transform centerPosition, float minRange, float maxRange)
        {
            int angle = Random.Range(0, 360);
            float distX = Random.Range(minRange, maxRange);
            float distY = Random.Range(minRange, maxRange);
            float posX = Mathf.Cos(angle * Mathf.Deg2Rad) * distX;
            float posY = Mathf.Sign(angle * Mathf.Deg2Rad) * distY;
            Vector3 position = centerPosition.position + new Vector3(posX, posY);
            return position;
        }

        public static Color HexToColor(string color)
        {
            Color parsedColor;
            ColorUtility.TryParseHtmlString("#"+color, out parsedColor);

            return parsedColor;
        }

        public static List<MergeOptionResultData> GetMergeOptionResultDataList(Equipment selectedResultMergeEquipment)
        {
            EquipmentData selectedEquipmentData = selectedResultMergeEquipment.EquipmentData;
            string equipmentCode = selectedResultMergeEquipment.EquipmentData.MergedItemCode;
            EquipmentData resultEquipmentData = Manager.I.Data.EquipmentDataDict[equipmentCode];
            var mergeOptionResultDataList = new List<MergeOptionResultData>();
            if (selectedEquipmentData.AtkDmgBonus != 0)
            {
                MergeOptionResultData data = new MergeOptionResultData
                {
                    equipAbilityStatType = EquipAbilityStatType.ATK,
                    beforeValue = selectedEquipmentData.AtkDmgBonus,
                    afterValue = resultEquipmentData.AtkDmgBonus
                };
                
                mergeOptionResultDataList.Add(data);
            }

            if (selectedEquipmentData.MaxHpBonus != 0)
            {
                MergeOptionResultData data = new MergeOptionResultData
                {
                    equipAbilityStatType = EquipAbilityStatType.HP,
                    beforeValue = selectedEquipmentData.MaxHpBonus,
                    afterValue = resultEquipmentData.MaxHpBonus
                };
                
                mergeOptionResultDataList.Add(data);
            }
            
            MergeOptionResultData levelData = new MergeOptionResultData
            {
                equipAbilityStatType = EquipAbilityStatType.Level,
                beforeValue = selectedEquipmentData.MaxLevel,
                afterValue = resultEquipmentData.MaxLevel
            };
            mergeOptionResultDataList.Add(levelData);
            
            MergeOptionResultData gradeData = new MergeOptionResultData
            {
                equipAbilityStatType = EquipAbilityStatType.Grade,
                beforeValue = (int) selectedEquipmentData.EquipmentGrade,
                afterValue = (int) resultEquipmentData.EquipmentGrade
            };
            mergeOptionResultDataList.Add(gradeData);

            return mergeOptionResultDataList;
        }

        public static TimeSpan GetOfflineRewardTime(DateTime lastOfflineGetRewardTime)
        {
            DateTime dateTime = lastOfflineGetRewardTime;
            TimeSpan timeSpan = DateTime.UtcNow - dateTime;
            if (timeSpan > TimeSpan.FromHours(24))
            {
                return TimeSpan.FromHours(23.9);
            }

            return timeSpan;
        }
    }
}
