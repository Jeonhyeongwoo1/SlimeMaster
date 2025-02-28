using System;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Interface;
using UnityEngine;

namespace SlimeMaster.Model
{
    public class TimeDataModel : IModel
    {
        public int StaminaCount
        {
            get => PlayerPrefs.GetInt(nameof(StaminaCount), 3);
            set => PlayerPrefs.SetInt(nameof(StaminaCount), value);
        }

        public void Initialize(DateTime lastLoginTime)
        {
            // PlayerPrefs.DeleteAll();
            if ((DateTime.UtcNow - lastLoginTime).TotalHours < 24)
            {
                return;
                // StaminaCount = PlayerPrefs.GetInt(nameof(StaminaCount), 3);
            }

            StaminaCount = 3;
        }

        public bool IsPossibleGetReward(DateTime lastOfflineGetRewardTime)
        {
            TimeSpan timeSpan = Utils.GetOfflineRewardTime(lastOfflineGetRewardTime);
            bool isPossibleReward = timeSpan.TotalMinutes > Const.MIN_OFFLINE_REWARD_MINUTE;
            return isPossibleReward;
        }
    }
}