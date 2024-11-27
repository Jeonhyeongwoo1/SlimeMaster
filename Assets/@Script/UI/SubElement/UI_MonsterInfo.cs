using SlimeMaster.Enum;
using SlimeMaster.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.InGame.View
{
    public class UI_MonsterInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Slider _slider;
        [SerializeField] private MonsterType _monsterType;

        private void OnEnable()
        {
            Managers.Manager.I.Event.AddEvent(GameEventType.TakeDamageEliteOrBossMonster, OnChangedRatio);
        }

        private void OnDisable()
        {
            Managers.Manager.I.Event.RemoveEvent(GameEventType.TakeDamageEliteOrBossMonster, OnChangedRatio);
        }
        
        public void UpdateMonsterInfo(string name, float ratio)
        {
            _nameText.text = name;
            _slider.value = ratio;
        }

        public void OnChangedRatio(object value)
        {
            _slider.value = (float)value;
        }
    }
}