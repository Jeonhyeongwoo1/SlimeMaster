using System.Collections.Generic;
using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Input;
using SlimeMaster.InGame.Popup;
using SlimeMaster.InGame.Skill;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.UISubItemElement;
using SlimeMaster.View;
using TMPro;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
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
        [SerializeField] private UI_SkillList _uiSkillList;
        [SerializeField] private RectTransform _soulShopRectTransform;
        [SerializeField] private Button _upgradeSkillButton;
        [SerializeField] private Button _soulShopCloseButton;
        [SerializeField] private Button _soulShopBGButton;
        [SerializeField] private Transform _skillSlotGroupTransform;
        [SerializeField] private GameObject _OwnBattleSkillInfoObject;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _staticsButton;
        [SerializeField] private RectTransform _soulIconRectTrasnform;
        [SerializeField] private TextMeshProUGUI _soulAmountText;
        [SerializeField] private List<UI_SupportCardItem> _uiSupportCardItemList;
        [SerializeField] private Button _resetSupportSkillCardButton;
        [SerializeField] private TextMeshProUGUI _supportSkillCardCount;
        [SerializeField] private Button _showSupportSkillListButton;
        [SerializeField] private Transform _supprotSkillGroupTransform;
        
        private List<UI_SkillSlotItem> _uiSkillSlotItemList;
        private bool _isShowSupportSkillList;
        
        public override void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }

            uiGameStageInfoPanel.Initialize(OnPauseGame);
            
            Managers.Manager.I.Event.AddEvent(GameEventType.SpawnedBoss, OnSpawnedBoss);
            Managers.Manager.I.Event.AddEvent(GameEventType.EndWave, OnWaveEnd);
            Managers.Manager.I.Event.AddEvent(GameEventType.LearnSkill, OnLearnSkill);
            Managers.Manager.I.Event.AddEvent(GameEventType.PurchaseSupportSkill, OnPurchaseSupportSkill);

            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.CurrentExpRatio
                .Subscribe(OnChangedPlayerExp)
                .AddTo(this);

            playerModel.CurrentLevel
                .Subscribe(OnChangedCurrentLevel)
                .AddTo(this);

            playerModel.SoulAmount.Subscribe(OnUpdateSoulAmount).AddTo(this);

            _showSupportSkillListButton.OnPointerClickAsObservable().Subscribe(x => OnShowSupportSkillList()).AddTo(this);
            _soulShopBGButton.OnPointerClickAsObservable().Subscribe(x => OnCloseSoulShop()).AddTo(this);
            _soulShopCloseButton.OnPointerClickAsObservable().Subscribe(x => OnCloseSoulShop()).AddTo(this);
            _upgradeSkillButton.OnPointerClickAsObservable().Subscribe(v => OnOpenSoulShop()).AddTo(this);
            _pauseButton.OnPointerClickAsObservable().Subscribe(x => OnOpenPausePopup()).AddTo(this);
            _staticsButton.OnPointerClickAsObservable().Subscribe(x => OnOpenStaticsPopup()).AddTo(this);
            _resetSupportSkillCardButton.OnPointerClickAsObservable().Subscribe(x => OnResetSupportSkillCard()).AddTo(this);
            
            IsInitialize = true;
        }
        
        private void OnShowSupportSkillList()
        {
            _isShowSupportSkillList = !_isShowSupportSkillList;
            if (_isShowSupportSkillList)
            {
                _OwnBattleSkillInfoObject.SetActive(false);
                _soulShopRectTransform.anchoredPosition = new Vector2(0, 1100);
            }
            else
            {
                _soulShopRectTransform.anchoredPosition = new Vector2(0, 580);
                _soulShopRectTransform.gameObject.SetActive(true);
                _soulShopCloseButton.gameObject.SetActive(true);
                _soulShopBGButton.gameObject.SetActive(true);
                _OwnBattleSkillInfoObject.SetActive(true);
            }
        }

        private List<UI_SupportSkillItem> _uiSupportSkillItemList = new();
        
        private void OnPurchaseSupportSkill(object value)
        {
            var supportSkillList = (List<SupportSkill>)value;
            foreach (UI_SupportSkillItem uiSupportSkillItem in _uiSupportSkillItemList)
            {
                uiSupportSkillItem.Release();
            }
            
            _uiSupportSkillItemList.Clear();
            foreach (SupportSkill supportSkill in supportSkillList)
            {
                GameObject prefab = Managers.Manager.I.Resource.Instantiate(nameof(UI_SupportSkillItem));
                var skillItem = prefab.GetComponent<UI_SupportSkillItem>();
                skillItem.SetInfo(supportSkill.SupportSkillData, _supprotSkillGroupTransform);
                _uiSupportSkillItemList.Add(skillItem);
            }

            _supportSkillCardCount.text = supportSkillList.Count.ToString();
        }

        private void OnResetSupportSkillCard()
        {
            bool isSuccess = Managers.Manager.I.Object.TryResetSupportSkillList();
            if (!isSuccess)
            {
                return;    
            }

            SetSupportSkillInfo(Managers.Manager.I.Object.Player.SkillBook.CurrentSupportSkillDataList);
        }

        private void OnOpenStaticsPopup()
        {
            Managers.Manager.I.UI.OpenPopup<UI_TotalDamagePopup>();
        }

        private void OnOpenPausePopup()
        {
            Time.timeScale = 0;
            Managers.Manager.I.UI.OpenPopup<UI_PausePopup>();
        }

        public void UpdateSkillSlotItem(List<BaseSkill> skillList)
        {
            if (_uiSkillSlotItemList == null || _uiSkillSlotItemList.Count == 0)
            {
                _uiSkillSlotItemList = new List<UI_SkillSlotItem>(Const.MAX_SKILL_COUNT);
                for (int i = 0; i < Const.MAX_SKILL_COUNT; i++)
                {
                    GameObject prefab = Managers.Manager.I.Resource.Instantiate(nameof(UI_SkillSlotItem), false);
                    var slotItem = prefab.GetComponent<UI_SkillSlotItem>();
                    Transform tran = slotItem.transform;
                    tran.SetParent(_skillSlotGroupTransform);
                    tran.localScale = Vector3.one;
                    tran.localPosition = Vector3.zero;
                    slotItem.gameObject.SetActive(true);
                    _uiSkillSlotItemList.Add(slotItem);
                }
            }

            for (var i = 0; i < _uiSkillSlotItemList.Count; i++)
            {
                if (i >= skillList.Count)
                {
                    _uiSkillSlotItemList[i].gameObject.SetActive(false);
                }
                else
                {
                    BaseSkill skill = skillList[i];
                    Sprite sprite = Managers.Manager.I.Resource.Load<Sprite>(skill.SkillData.IconLabel);
                    _uiSkillSlotItemList[i].UpdateUI(sprite, skill.CurrentLevel);
                }
            }
        }

        private void OnSpawnedBoss(object value)
        {
            _bossSpawnAlarmObject.SetActive(true);
            _monsterSpawnAlarmObject.SetActive(false);

            if (_bossSpawnAlarmObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(canvasGroup.DOFade(1, 0.5f));
                sequence.SetLoops(8, LoopType.Yoyo);
                sequence.OnComplete(() => _bossSpawnAlarmObject.gameObject.SetActive(false));
            }
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
                sequence.SetLoops(8, LoopType.Yoyo);
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
            }
            else if (monsterType == MonsterType.Elete)
            {
                _eliteMonsterInfo.gameObject.SetActive(false);
            }
        }

        public void OnOpenSoulShop()
        {
            _soulShopRectTransform.anchoredPosition = new Vector2(0, 580);
            _soulShopRectTransform.gameObject.SetActive(true);
            _soulShopCloseButton.gameObject.SetActive(true);
            _soulShopBGButton.gameObject.SetActive(true);
            _OwnBattleSkillInfoObject.SetActive(true);
            InputHandler.onActivateInputHandlerAction.Invoke(false);
            SetSupportSkillInfo(Managers.Manager.I.Object.Player.SkillBook.CurrentSupportSkillDataList);
            Time.timeScale = 0;
        }

        public void OnCloseSoulShop()
        {
            _soulShopRectTransform.DOKill();
            _soulShopRectTransform.DOAnchorPos(Vector2.zero, 0.3f);
            _soulShopCloseButton.gameObject.SetActive(false);
            _soulShopBGButton.gameObject.SetActive(false);
            _OwnBattleSkillInfoObject.SetActive(false);
            InputHandler.onActivateInputHandlerAction.Invoke(true);
            Time.timeScale = 1;
        }
        
        private void OnLearnSkill(object value)
        {
            SkillData skillData = (SkillData)value;
            var iconLabel = Managers.Manager.I.Resource.Load<Sprite>(skillData.IconLabel);
            _uiSkillList.UpdateSkillInfo(iconLabel);
        }
        
        public void SetSupportSkillInfo(List<SupportSkillData> supportSkillDataList)
        {
            for (int i = 0; i < Const.SUPPORT_ITEM_USEABLECOUNT; i++)
            {
                if (supportSkillDataList.Count <= i)
                {
                    _uiSupportCardItemList[i].gameObject.SetActive(false);
                    continue;
                }
                
                var supportSkillData = supportSkillDataList[i];
                _uiSupportCardItemList[i].SetInfo(supportSkillData);
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

        public Transform GetSoulIconTransform()
        {
            return _soulIconRectTrasnform;
        }

        private void OnUpdateSoulAmount(int soulAmount)
        {
            _soulAmountText.text = soulAmount.ToString();
        }
    }
}
