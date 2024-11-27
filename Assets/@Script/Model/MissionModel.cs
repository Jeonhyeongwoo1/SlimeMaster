using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using UniRx;
using UnityEngine;

namespace SlimeMaster.Model
{
    public class MissionModel : IModel
    {
        public ReactiveProperty<bool> IsPossibleGetReward = new();
        public List<MissionModelData> missionDataList;

        public void SetMissionData(DBMissionContainerData dbMissionContainerData)
        {
            missionDataList ??= new List<MissionModelData>();
            if (missionDataList.Count > 0)
            {
                missionDataList.Clear();
            }
            
            foreach (DBMissionData dbMissionData in dbMissionContainerData.DBDailyMissionDataList)
            {
                MissionModelData missionModelData = new MissionModelData
                {
                    missionID = dbMissionData.MissionId,
                    accumulatedValue =
                    {
                        Value = dbMissionData.AccumulatedValue
                    },
                    isGet =
                    {
                        Value = dbMissionData.IsGet
                    }
                };
                
                missionDataList.Add(missionModelData);
            }
            
            foreach (DBMissionData dbMissionData in dbMissionContainerData.DBDailyMissionDataList)
            {
                MissionData missionData = Manager.I.Data.MissionDataDict[dbMissionData.MissionId];
                bool isPossibleGetReward = missionData.MissionTargetValue <= dbMissionData.AccumulatedValue && !dbMissionData.IsGet;
                IsPossibleGetReward.Value = isPossibleGetReward;
                if (isPossibleGetReward)
                {
                    break;
                }
            }
        }

        public MissionModelData GetMissionData(int missionId)
        {
            return missionDataList.Find(x => x.missionID == missionId);
        }
    }

    [Serializable]
    public class MissionModelData
    {
        public int missionID;
        public ReactiveProperty<int> accumulatedValue = new();
        public ReactiveProperty<bool> isGet = new();
    }
}