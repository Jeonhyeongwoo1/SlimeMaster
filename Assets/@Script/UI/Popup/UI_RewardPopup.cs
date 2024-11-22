using SlimeMaster.Common;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

public class UI_RewardPopup : BasePopup
{
    public Transform RewardItemScrollContentObject => _rewardItemScrollContentObject;
    [SerializeField] private Transform _rewardItemScrollContentObject;

    public void ReleaseRewardItemScrollContentChildComponent()
    {
        var childs = Utils.GetChildComponent<UI_MaterialItem>(_rewardItemScrollContentObject);
        if (childs == null)
        {
            return;
        }
        
        foreach (UI_MaterialItem item in childs)
        {
            item.Release();
        }
    }
    
    public void UpdateUI()
    {
    }
}
