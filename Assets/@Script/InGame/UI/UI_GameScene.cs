using DG.Tweening;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Model;
using SlimeMaster.View;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace SlimeMaster.InGame.View
{
    public class UI_GameScene : BaseUI
    {
        [SerializeField] private UI_GameStageInfoPanel uiGameStageInfoPanel;
        [SerializeField] private Slider _playerExpSlider;
        [SerializeField] private TextMeshProUGUI _playerLvelText;
        [SerializeField] private UI_MonsterInfo _eliteMonsterInfo;
        [SerializeField] private UI_MonsterInfo _bossMonsterInfo;
        [SerializeField] private GameObject _monsterSpawnAlarmObject;
        [SerializeField] private GameObject _bossSpawnAlarmObject;
        
        public override void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }
            
            uiGameStageInfoPanel.Initialize(OnPauseGame);
            
            GameManager.I.Event.AddEvent(GameEventType.SpawnedBoss, OnSpawnedBoss);
            GameManager.I.Event.AddEvent(GameEventType.EndWave, OnWaveEnd);

            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.CurrentExpRatio
                .Subscribe(OnChangedPlayerExp)
                .AddTo(this);

            playerModel.CurrentLevel
                .Subscribe(OnChangedCurrentLevel)
                .AddTo(this);
    
            IsInitialize = true;
        }

        private void OnSpawnedBoss(object value)
        {
            _bossSpawnAlarmObject.SetActive(true);
            _monsterSpawnAlarmObject.SetActive(false);
        }

        private void OnWaveEnd(object value)
        {
            _bossSpawnAlarmObject.SetActive(false);
            _monsterSpawnAlarmObject.SetActive(true);

            if (_monsterSpawnAlarmObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(canvasGroup.DOFade(1, 0.5f));
                sequence.SetLoops(2, LoopType.Yoyo);
                sequence.OnComplete(() => _monsterSpawnAlarmObject.gameObject.SetActive(false));
            }
        }

        public void ShowMonsterInfo(MonsterType monsterType, string monsterName, float ratio)
        {
            if (monsterType == MonsterType.Boss)
            {
                _bossMonsterInfo.gameObject.SetActive(true);
                _bossMonsterInfo.UpdateMonsterInfo(monsterName, ratio);
            }
            else if(monsterType == MonsterType.Elete)
            {
                _eliteMonsterInfo.gameObject.SetActive(true);
                _eliteMonsterInfo.UpdateMonsterInfo(monsterName, ratio);
            }
        }

        public void HideMonsterInfo(MonsterType monsterType)
        {
            if (monsterType == MonsterType.Boss)
            {
                _bossMonsterInfo.gameObject.SetActive(false);
            }else if (monsterType == MonsterType.Elete)
            {
                _eliteMonsterInfo.gameObject.SetActive(false);
            }
        }

        private void OnChangedCurrentLevel(int value)
        {
            _playerLvelText.text = value.ToString();
        }

        private void OnChangedPlayerExp(float value)
        {
            _playerExpSlider.value = value;
        }

        private void OnPauseGame()
        {
            
        }
    }
}
