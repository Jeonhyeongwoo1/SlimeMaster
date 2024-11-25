using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.Server
{
    public static class ServerMissionHelper
    {
        public static DBMissionContainerData InitializeMissionData(DataManager dataManager)
        {
            var dbMissionContainerData = new DBMissionContainerData
            {
                DBDailyMissionDataList = new List<DBMissionData>()
            };

            foreach (var (key, missionData) in dataManager.MissionDataDict)
            {
                DBMissionData dbMissionData = new DBMissionData
                {
                    MissionId = missionData.MissionId,
                    MissionTarget = (int)missionData.MissionTarget,
                    MissionType = (int)missionData.MissionType,
                    AccumulatedValue = 0,
                    IsGet = false
                };
                
                dbMissionContainerData.DBDailyMissionDataList.Add(dbMissionData);
            }
            
            return dbMissionContainerData;
        }

        public static DBMissionContainerData UpdateMissionAccumulatedValue(MissionTarget missionTarget,
            DBMissionContainerData dbMissionContainerData, int addValue, DataManager dataManager)
        {
            dbMissionContainerData ??= InitializeMissionData(dataManager);
            DBMissionData dbMissionData =
                dbMissionContainerData.DBDailyMissionDataList.Find(v => v.MissionTarget == (int)missionTarget);

            if (dbMissionData == null || dbMissionData.IsGet)
            {
                return dbMissionContainerData;
            }

            dbMissionData.AccumulatedValue += addValue;
            return dbMissionContainerData;
        }

        public static bool TryMissionClear(MissionTarget missionTarget, int addValue,
            DBMissionContainerData dbMissionContainerData, DataManager dataManager, ref int clearRewardItem,
            ref int rewardValue)
        {
            DBMissionData dbMissionData =
                dbMissionContainerData.DBDailyMissionDataList.Find(v => v.MissionTarget == (int)missionTarget);

            if (dbMissionData.IsGet)
            {
                return false;
            }

            dbMissionData.AccumulatedValue += addValue;
            MissionData missionData = dataManager.MissionDataDict[dbMissionData.MissionId];
            if (missionData.MissionTargetValue > dbMissionData.AccumulatedValue)
            {
                return false;
            }

            dbMissionData.IsGet = true;
            clearRewardItem = missionData.ClearRewardItmeId;
            rewardValue = missionData.RewardValue;
            return true;
        }
    }
}