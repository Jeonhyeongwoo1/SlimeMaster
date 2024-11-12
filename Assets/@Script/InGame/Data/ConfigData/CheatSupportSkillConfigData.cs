using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using Sirenix.OdinInspector;
using SlimeMaster.InGame.Enum;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CheatSupportSkillConfigData", order = 1)]
public class CheatSupportSkillConfigData : ScriptableObject
{
    [OnValueChanged(nameof(OnSoulAmountChanged))]
    public int soulAmount;

    public SupportSkillName supportSkillName;
    public SupportSkillGrade supportSkillGrade;

    [Button]
    public void AddSupportSkill()
    {
        foreach (var (key, value) in GameManager.I.Data.SupportSkillDataDict)
        {
            if (value.SupportSkillName == supportSkillName && value.SupportSkillGrade == supportSkillGrade)
            {
                if (value.IsPurchased)
                {
                    return;
                }
                
                bool isSuccess = GameManager.I.Object.Player.TryPurchaseSupportSkill(key);
                if (!isSuccess)
                {
                    Debug.LogError($"Failed purchase support skill {key} / name {supportSkillName}");
                }

                break;
            }
        }
    }
    
    public void OnSoulAmountChanged(int soulAmount)
    {
        GameManager.I.Object.Player.SoulAmount = soulAmount;
    }
}
