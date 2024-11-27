using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using UniRx;
using UnityEngine.Serialization;

namespace SlimeMaster.Model
{
    public class AchievementModel : IModel
    {
        public ReactiveProperty<bool> IsPossibleGetReward = new();
        
        public List<AchievementModelData> AchievementModelDataList { get; set; }
        public List<DBAchievementData> RewardedAchievementModelDataList { get; set; }

        public void Initialize(DBAchievementContainerData dbAchievementContainerData)
        {
            AchievementModelDataList ??= new List<AchievementModelData>();
            if (AchievementModelDataList.Count > 0)
            {
                AchievementModelDataList.Clear();
            }
            
            foreach (DBAchievementData dbAchievementData in dbAchievementContainerData.DBAchievementDataList)
            {
                AchievementModelData newData = new AchievementModelData
                {
                    achievementId = dbAchievementData.AchievementId,
                    accumulatedValue =
                    {
                        Value = dbAchievementData.AccumulatedValue
                    },
                    missionTarget = (MissionTarget)dbAchievementData.MissionTarget
                };

                AchievementModelDataList.Add(newData);
            }

            RewardedAchievementModelDataList ??= new List<DBAchievementData>();
            if (RewardedAchievementModelDataList.Count > 0)
            {
                RewardedAchievementModelDataList.Clear();
            }

            if (dbAchievementContainerData.DBRewardAchievementDataList != null)
            {
                foreach (DBAchievementData dbRewardAchievementData in dbAchievementContainerData.DBRewardAchievementDataList)
                {
                    DBAchievementData newData = new DBAchievementData()
                    {
                        AchievementId = dbRewardAchievementData.AchievementId
                    };
                
                    RewardedAchievementModelDataList.Add(newData);
                }
            }
            
            foreach (DBAchievementData dbAchievementData in dbAchievementContainerData.DBAchievementDataList)
            {
                AchievementData achievementData = Manager.I.Data.AchievementDataDict[dbAchievementData.AchievementId];

                bool isPossibleGetReward = achievementData.MissionTargetValue <= dbAchievementData.AccumulatedValue;
                IsPossibleGetReward.Value = isPossibleGetReward;
                if (isPossibleGetReward)
                {
                    break;
                }
            }
        }

        public int FindAccumulatedValue(int achievementId)
        {
             var data = AchievementModelDataList.Find(v=> v.achievementId == achievementId);
             return data == null ? 0 : data.accumulatedValue.Value;
        }

        public bool HasAchievementModelData(int achievementId)
        {
            var data = AchievementModelDataList.Find(v=> v.achievementId == achievementId);
            return data != null;
        }

        public bool IsGetReward(int achievementId)
        {
            if (RewardedAchievementModelDataList == null)
            {
                return false;
            }
            
            return RewardedAchievementModelDataList.Find(v=> v.AchievementId == achievementId) != null;
        }
    }
    
    [Serializable]
    public class AchievementModelData
    {
        public int achievementId;
        public MissionTarget missionTarget;
        public ReactiveProperty<int> accumulatedValue = new();
    }
}