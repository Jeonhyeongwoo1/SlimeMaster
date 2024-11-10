using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_SkillSlotItem : MonoBehaviour
    {
        [SerializeField] private Image _skillImage;
        [SerializeField] private List<GameObject> _skillLevelStarObjectList;

        public void UpdateUI(Sprite sprite, int level)
        {
            _skillImage.sprite = sprite;
            for (var i = 0; i < _skillLevelStarObjectList.Count; i++)
            {
                _skillLevelStarObjectList[i].SetActive(i < level);
            }
            
            gameObject.SetActive(true);
        }
    }
}