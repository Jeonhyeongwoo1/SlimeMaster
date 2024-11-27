using UnityEngine;
using Sirenix.OdinInspector;
using SlimeMaster.Enum;
using SlimeMaster.Managers;

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
        foreach (var (key, value) in Manager.I.Data.SupportSkillDataDict)
        {
            if (value.SupportSkillName == supportSkillName && value.SupportSkillGrade == supportSkillGrade)
            {
                if (value.IsPurchased)
                {
                    return;
                }
                
                bool isSuccess = Manager.I.Object.Player.TryPurchaseSupportSkill(key);
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
        Manager.I.Object.Player.SoulAmount = soulAmount;
    }
}
