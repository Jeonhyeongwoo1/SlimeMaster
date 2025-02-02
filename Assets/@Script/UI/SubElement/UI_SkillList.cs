using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.InGame.View
{
    public class UI_SkillList : MonoBehaviour
    {
        [SerializeField] private List<Image> _skillImageList;

        public void UpdateSkillInfo(Sprite sprite)
        {
            Debug.Log(transform.name);
            var image = _skillImageList.Find(v => !v.enabled);
            if (image)
            {
                image.enabled = true;
                image.sprite = sprite;
            }
        }
    }
}