using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Script.InGame.UI.Popup;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.Input;
using SlimeMaster.InGame.Popup;
using SlimeMaster.InGame.Skill;
using SlimeMaster.InGame.View;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class PlayerController : CreatureController
    {
        #region PlayerStat
        public float CriRate
        {
            get => _playerStat.criRate;
            set => _playerStat.criRate = _playerStatForView.criRate = value;
        }

        public float MaxHPBonus
        {
            get => _playerStat.maxHPBonus;
            set => _playerStat.maxHPBonus = _playerStatForView.maxHPBonus = value;
        }

        public float EXPBonusRate
        {
            get => _playerStat.expBonusRate;
            set => _playerStat.expBonusRate = _playerStatForView.expBonusRate = value;
        }

        public float SoulBonusRate
        {
            get => _playerStat.soulBonusRate;
            set => _playerStat.soulBonusRate = _playerStatForView.soulBonusRate = value;
        }

        public float DamageReduction
        {
            get => _playerStat.damageReduction;
            set => _playerStat.damageReduction = _playerStatForView.damageReduction = value;
        }

        public float DefRate
        {
            get => _playerStat.defRate;
            set => _playerStat.defRate = _playerStatForView.defRate = value;
        }

        public float AttackBonusRate
        {
            get => _playerStat.attackBonusRate;
            set => _playerStat.attackBonusRate = _playerStatForView.attackBonusRate = value;
        }

        public float MoveSpeedRate
        {
            get => _playerStat.moveSpeedRate;
            set => _playerStat.moveSpeedRate = _playerStatForView.moveSpeedRate = value;
        }

        public float Healing
        {
            get => _playerStat.healing;
            set => _playerStat.healing = _playerStatForView.healing = value;
        }

        public float HealBonusRate
        {
            get => _playerStat.healBonusRate;
            set => _playerStat.healBonusRate = _playerStatForView.healBonusRate = value;
        }

        public float HPRegen
        {
            get => _playerStat.hpRegen;
            set => _playerStat.hpRegen = _playerStatForView.hpRegen = value;
        }

        public float CriticalDamage
        {
            get => _playerStat.criticalDamage == 0 ?  1.5f : _playerStat.criticalDamage;
            set => _playerStat.criticalDamage = _playerStatForView.criticalDamage = value;
        }

        public float MagneticRange
        {
            get => _playerStat.magneticRange;
            set => _playerStat.magneticRange = _playerStatForView.magneticRange = value;
        }

        public float Resurrection
        {
            get => _playerStat.resurrection;
            set => _playerStat.resurrection = _playerStatForView.resurrection = value;
        }

        public override float MaxHP
        {
            get => _playerStat.maxHP;
            set => _playerStat.maxHP = _playerStatForView.maxHP = value;
        }

        public override float HP
        {
            get => _playerStat.HP;
            set => _playerStat.HP = _playerStatForView.HP = value;
        }

        public int Level
        {
            get => _playerStat.level;
            set => _playerStat.level = _playerStatForView.level = value;
        }

        private PlayerStat _playerStat;

        [SerializeField] private PlayerStat _playerStatForView;

        #endregion
        
        public int Layer => _layer;

        public float CurrentExp
        {
            get => _playerModel.CurrentExp.Value;
            set
            {
                _playerModel.CurrentExp.Value += Mathf.RoundToInt(value * EXPBonusRate);
                var levelData = Managers.Manager.I.Data.LevelDataDict[_playerModel.CurrentLevel.Value];
                _playerModel.CurrentExpRatio.Value = (float)_playerModel.CurrentExp.Value / levelData.TotalExp;
                if (_playerModel.CurrentExp.Value >= levelData.TotalExp)
                {
                    OnLevelUp();
                    _playerModel.CurrentExp.Value -= levelData.TotalExp;
                }

                Managers.Manager.I.GameContinueData.playerExp = _playerModel.CurrentExp.Value;
            }
        }

        public int SoulAmount
        {
            get => _playerModel.SoulAmount.Value;
            set
            {
                _playerModel.SoulAmount.Value = value;
                Managers.Manager.I.GameContinueData.soulAmount = value;
            }
        }

        public override float AttackDamage => (_creatureData.Atk + _attackDamage) * _creatureData.AtkRate * AttackBonusRate;

        protected override float MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                _moveSpeed = value;
                _playerMove.SetMoveSpeed(_moveSpeed);
            }
        }

        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private Transform _indicatorSpriteTransform;
        [SerializeField] private PlayerMove _playerMove;

        private int _layer;
        private Vector2 _inputVector;
        private bool _isInit;
        private float _moveSpeed;
        private PlayerModel _playerModel;

        private CancellationTokenSource _recoveryHPCts;

        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            GameContinueData gameContinueData = Managers.Manager.I.GameContinueData;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            _playerStat = gameContinueData.playerStat; 
            base.Initialize(creatureData, sprite, skillDataList);
            _isInit = true;
            _creatureType = CreatureType.Player;
            
            _playerMove.Initialize(_rigidbody, _indicatorTransform, _spriteRenderer);
            InitCreatureStat();
            InitializeEquipment();

            if (gameContinueData.IsContinue)
            {
                foreach (SkillData baseSkill in gameContinueData.skillList)
                {
                    _skillBook.UpgradeOrAddSkill(baseSkill);
                }

                foreach (SupportSkillData supportSkillData in gameContinueData.supportSkillDataList)
                {
                    _skillBook.AddSupportSkill(supportSkillData);
                }
            }
            
            _skillBook.CurrentSupportSkillDataList = _skillBook.GetRecommendSupportSkillDataList();
            MoveSpeed = creatureData.MoveSpeed * creatureData.MoveSpeedRate;
            List<BaseSkill> skillList = _skillBook.ActivateSkillList;
            UIManager uiManager = Managers.Manager.I.UI;
            var gamesceneUI = uiManager.SceneUI as UI_GameScene;
            gamesceneUI.UpdateSkillSlotItem(skillList);
            gameObject.SetActive(true);
        }

        private void InitializeEquipment()
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            Equipment equipment =
                userModel.EquippedItemDataList.Value.Find(v => v.GetEquipmentType() == EquipmentType.Weapon);
            if (equipment != null)
            {
                SkillData basicSkillData = Managers.Manager.I.Data.SkillDict[equipment.EquipmentData.BasicSkill];
                if (_skillBook.IsLearnSkill(basicSkillData))
                {
                    return;
                }
                
                _skillBook.UpgradeOrAddSkill(basicSkillData);

                SupportSkillData uncommonSkill;
                SupportSkillData rareSkill;
                SupportSkillData epicSkill;
                SupportSkillData legendSkill;
                //등급별 서포트 스킬
                foreach (Equipment equip in userModel.EquippedItemDataList.Value)
                {
                    switch (equip.EquipmentData.EquipmentGrade)
                    {
                        case EquipmentGrade.Uncommon:
                            //UncommonGradeSkill 추가
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(
                                    equip.EquipmentData.UncommonGradeSkill, out uncommonSkill))
                                _skillBook.AddSupportSkill(uncommonSkill);
                            break;
                        case EquipmentGrade.Rare:
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(
                                    equip.EquipmentData.UncommonGradeSkill, out uncommonSkill))
                                _skillBook.AddSupportSkill(uncommonSkill);
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(equip.EquipmentData.RareGradeSkill,
                                    out rareSkill))
                                _skillBook.AddSupportSkill(rareSkill);
                            break;
                        case EquipmentGrade.Epic:
                        case EquipmentGrade.Epic1:
                        case EquipmentGrade.Epic2:
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(
                                    equip.EquipmentData.UncommonGradeSkill, out uncommonSkill))
                                _skillBook.AddSupportSkill(uncommonSkill);
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(equip.EquipmentData.RareGradeSkill,
                                    out rareSkill))
                                _skillBook.AddSupportSkill(rareSkill);
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(equip.EquipmentData.EpicGradeSkill,
                                    out epicSkill))
                                _skillBook.AddSupportSkill(epicSkill);
                            break;
                        case EquipmentGrade.Legendary:
                        case EquipmentGrade.Legendary1:
                        case EquipmentGrade.Legendary2:
                        case EquipmentGrade.Legendary3:
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(
                                    equip.EquipmentData.UncommonGradeSkill, out uncommonSkill))
                                _skillBook.AddSupportSkill(uncommonSkill);
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(equip.EquipmentData.RareGradeSkill,
                                    out rareSkill))
                                _skillBook.AddSupportSkill(rareSkill);
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(equip.EquipmentData.EpicGradeSkill,
                                    out epicSkill))
                                _skillBook.AddSupportSkill(epicSkill);
                            if (Managers.Manager.I.Data.SupportSkillDataDict.TryGetValue(
                                    equip.EquipmentData.LegendaryGradeSkill, out legendSkill))
                                _skillBook.AddSupportSkill(legendSkill);
                            break;
                    }
                }
            }
        }

        protected override void InitCreatureStat(bool isFullHP = true)
        {
            MaxHP = _creatureData.MaxHp;
            if (isFullHP)
            {
                HP = MaxHP;
            }

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            var (equip_hp, equip_attack) = userModel.GetEquipmentBonus();
            MaxHP += equip_hp;
            MaxHP *= MaxHPBonus;
            
            // _attackDamage += equip_attack;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            _playerModel.CurrentLevel.Value = Level;

            // HP = MaxHP = 10000;
            // SoulAmount = 1000;
        }

        private void Start()
        {
            _layer = gameObject.layer;
            TryGetComponent(out _rigidbody);
            RecoveryHpAsync().Forget();
        }

        private void OnLevelUp()
        {
            /*
             *  스킬리스트 정리
             *  총 6개의 스킬을 얻음.
             *  6개의 스킬을 가지고 있으면
             *      만랩이 아닌 스킬중에서 3개를 선택
             *  아니면
             *  선택할 수 있는 스킬들 중에서 만랩이 아닌 스킬 3개를 선택
             */
            List<BaseSkill> skillList = _skillBook.GetRecommendSkillList();
            if (skillList.Count == 0)
            {
                Debug.Log($"recommend skill list {skillList.Count}");
                return;
            }
            
            var popup = Managers.Manager.I.UI.OpenPopup<UI_SkillSelectPopup>();
            popup.UpdateUI(skillList, _skillBook.ActivateSkillList);
            
            Time.timeScale = 0;
        }

        private async UniTaskVoid RecoveryHpAsync()
        {
            _recoveryHPCts = new CancellationTokenSource();
            while (!_recoveryHPCts.IsCancellationRequested)
            {
                Heal(HPRegen);

                try
                {
                    await UniTask.WaitForSeconds(1, cancellationToken: _recoveryHPCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogError($"error {nameof(RecoveryHpAsync)} message {e.Message}");
                    break;
                }
            }
        }
        private void OnEnable()
        {
            InputHandler.onInputAction += OnChangedInputVector;
            // InputHandler.onPointerUpAction += OnChangedInputVector;
            Managers.Manager.I.Event.AddEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
            Managers.Manager.I.Event.AddEvent(GameEventType.UpgradeOrAddNewSkill, OnUpgradeOrAddNewSkill);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Utils.SafeCancelCancellationTokenSource(ref _recoveryHPCts);
            InputHandler.onInputAction -= OnChangedInputVector;
            // InputHandler.onPointerUpAction -= OnChangedInputVector;
            Managers.Manager.I.Event.RemoveEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
            Managers.Manager.I.Event.RemoveEvent(GameEventType.UpgradeOrAddNewSkill, OnUpgradeOrAddNewSkill);
        }

        private void OnChangedInputVector(Vector2 input)
        {
            _inputVector = input;

            if (_inputVector == Vector2.zero)
            {
                if (_indicatorTransform.gameObject.activeSelf)
                {
                    _indicatorTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!_indicatorTransform.gameObject.activeSelf)
                {
                    _indicatorTransform.gameObject.SetActive(true);
                }
            }
            
            _playerMove.SetDirection(_inputVector);
        }
        
        private void OnUpgradeOrAddNewSkill(object value)
        {
            int skillId = (int)value;
            SkillData skill = Managers.Manager.I.Data.SkillDict[skillId];
            UpgradeOrAddSKill(skill);
            LevelUp();
            
            Managers.Manager.I.UI.ClosePopup();
            List<BaseSkill> skillList = _skillBook.ActivateSkillList;
            var gamesceneUI = Managers.Manager.I.UI.SceneUI as UI_GameScene;
            gamesceneUI.UpdateSkillSlotItem(skillList);

            Time.timeScale = 1;
        }
        
        public bool TryPurchaseSupportSkill(int supportSkillId)
        {
            SupportSkillData skillData = Managers.Manager.I.Data.SupportSkillDataDict[supportSkillId];
            if (SoulAmount < skillData.Price)
            {
                Debug.Log(
                    $"failed purchase skill soul amount {SoulAmount} / price {skillData.Price} / skill Id {supportSkillId}");
                return false;
            }

            SoulAmount -= (int) skillData.Price;
            Debug.Log($"{SoulAmount}/ {skillData.Price}");
            _skillBook.AddSupportSkill(skillData);
            var lockSupportSkillList = _skillBook._lockSupportSkillDataList;
            if (lockSupportSkillList.Contains(skillData))
            {
                lockSupportSkillList.Remove(skillData);
            }

            if (skillData.SupportSkillName == SupportSkillName.Healing)
            {
                Heal(skillData.HealRate);
                Managers.Manager.I.Event.Raise(GameEventType.PurchaseSupportSkill, _skillBook.ActivateSupportSkillDataList);
                return true;
            }

            if (skillData.SupportSkillType == SupportSkillType.General)
            {
                HPRegen += skillData.HpRegen;
                MaxHPBonus += skillData.HpRate;
                HealBonusRate += skillData.HealBonusRate;
                MagneticRange += skillData.MagneticRange;
                SoulBonusRate += skillData.SoulBonusRate;
                DamageReduction += skillData.DamageReduction;
                CriRate += skillData.CriRate;
                EXPBonusRate += skillData.ExpBonusRate;
                AttackBonusRate += skillData.AtkRate;
                DefRate += skillData.DefRate;
                MoveSpeedRate += skillData.MoveSpeedRate;
                CriticalDamage += skillData.CriDmg;
                UpdatePlayerStat();
            }
            else if (skillData.SupportSkillType == SupportSkillType.Special)
            {
                _skillBook.UpdateSkill(skillData);
            }

            Managers.Manager.I.Event.Raise(GameEventType.PurchaseSupportSkill, _skillBook.ActivateSupportSkillDataList);
            
            //Add effect

            return true;
        }

        private void UpdatePlayerStat(bool isFullHP = true)
        {
            InitCreatureStat(isFullHP);

            MaxHP *= MaxHPBonus;
            HP *= MaxHPBonus;
            MoveSpeed *= MoveSpeedRate;
            
            Debug.Log($"HP {HP} / MAXHP {MaxHP} / {MaxHPBonus}");
        }

        private void Update()
        {
            if (!_isInit || IsDeadState)
            {
                return;
            }

            GetDropItem();

            // if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            // {
            //     SkillData skillData = _skillBook.ActivateSkillList
            //         .Find(v => v.SkillType == SkillType.StormBlade).SkillData;
            //     _skillBook.UpgradeOrAddSkill(skillData);
            // }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnActivateAI();
            }
        }

        private void OnActivateAI()
        {
            var ai = gameObject.GetComponent<PlayerAI>();
            ai.Activate();
        }

        private void OnActivateDropItem(object value)
        {
            DropItemData dropItemData = (DropItemData)value;
            switch (dropItemData.DropItemType)
            {
                case DropableItemType.Potion:
                    if (Const.DicPotionAmount.TryGetValue(dropItemData.DataId, out float healAmount))
                    {
                        healAmount *= HealBonusRate;
                        Heal(healAmount);
                    }
                    break;
                case DropableItemType.DropBox:
                    var learnSkillPopup = Managers.Manager.I.UI.OpenPopup<UI_LearnSkillPopup>();
                    List<BaseSkill> skillList = _skillBook.GetRecommendSkillList(1);
                    if (skillList == null)
                    {
                        Debug.LogError("failed get skill list");
                        learnSkillPopup.ClosePopup();
                        return;
                    }
                    
                    learnSkillPopup.UpdateSKillItem(skillList[0]);
                    Time.timeScale = 1;
                    break;
            }
        }
        
        private void GetDropItem()
        {
            GridController grid = Managers.Manager.I.Game.CurrentMap.Grid;
            List<DropItemController> itemControllerList =
                grid.GetDropItem(transform.position, MagneticRange * Const.DEFAULT_MagneticRange);
            
            foreach (DropItemController item in itemControllerList)
            {
                grid.RemoveItem(item);
                Action callback = null;
                switch (item.DropableItemType)
                {
                    case DropableItemType.Gem:
                        callback = () =>
                        {
                            var gem = item as GemController;
                            int exp = gem.GetExp();
                            CurrentExp = exp;
                        };     
                        item.GetItem(transform, callback);
                        break;
                    case DropableItemType.DropBox:
                    case DropableItemType.Potion:
                    case DropableItemType.Magnet:
                    case DropableItemType.Bomb:
                        item.GetItem(transform, callback);
                        break;
                    case DropableItemType.Soul:
                        callback = () =>
                        {
                            var soul = item as SoulController;
                            SoulAmount += Mathf.RoundToInt(soul.GetSoulAmount() * SoulBonusRate);
                        };

                        var gameSceneUI = Managers.Manager.I.UI.SceneUI as UI_GameScene;
                        Transform tr = gameSceneUI.GetSoulIconTransform();
                        item.GetItem(tr, callback);
                        break;
                }
                
                Managers.Manager.I.Object.DroppedItemControllerList.Remove(item);
            }
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            if (IsDeadState)
            {
                return;
            }

            damage -= damage * DamageReduction;
            base.TakeDamage(damage, attacker);
            onHitReceived?.Invoke((int)HP, (int)_creatureData.MaxHp);
        }

        protected override async void Dead()
        {
            UpdateCreatureState(CreatureStateType.Dead);
            HP = 0;
            transform.DOKill();
            transform.DOScale(Vector3.zero, 0.3f);
            Managers.Manager.I.Event.Raise(GameEventType.DeadPlayer);
            _skillBook?.StopAllSkillLogic();
            _playerMove.SetStop(true);

            SupportSkill supportSkill = null;
            bool isPossibleResurrection = _skillBook.TryResurrection(ref supportSkill);
            if (isPossibleResurrection)
            {
                await DoResurrectionAsync(supportSkill);
            }
            else
            {
            }
        }

        //부활
        private async UniTask DoResurrectionAsync(SupportSkill supportSkill)
        {
            Heal(supportSkill.SupportSkillData.HealRate);
            MoveSpeedRate += supportSkill.SupportSkillData.MoveSpeedRate;
            AttackBonusRate += supportSkill.SupportSkillData.AtkRate;
            UpdatePlayerStat(false);
            
            _skillBook.UsedResurrectionSupportSkill(supportSkill);

            await UniTask.WaitForSeconds(1f, cancelImmediately: true);
            var prefab = Managers.Manager.I.Resource.Instantiate(Const.Revival, transform);
            prefab.SetActive(true);
            prefab.transform.position = transform.position;
            DOVirtual.DelayedCall(1, () => Managers.Manager.I.Pool.ReleaseObject(Const.Revival, prefab));
            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            UpdateCreatureState(CreatureStateType.Idle);
            _skillBook.UseAllSkillList(true, false, null);
            Managers.Manager.I.Event.Raise(GameEventType.ResurrectionPlayer);
        }

        public void UpgradeOrAddSKill(SkillData skillData)
        {
            _skillBook.UpgradeOrAddSkill(skillData);

            Managers.Manager.I.GameContinueData.skillList = _skillBook.ActivateSkillList.Select(x => x.SkillData).ToList();
        }

        public int GetActivateSkillLevel(int skillId)
        {
            return _skillBook.GetActivateSkillLevel(skillId);
        }
        
        public void LevelUp()
        {
            _playerModel.CurrentLevel.Value++;
            Managers.Manager.I.GameContinueData.playerLevel++;
            CurrentExp = _playerModel.CurrentExp.Value;

            List<SupportSkillData> supportSkillList = _skillBook.GetLevelSupportSkillDataList();
            if (supportSkillList.Count == 0)
            {
                return;
            }

            float moveRate = 0;
            float atkRate = 0;
            float criRate = 0;
            float cridmg = 0;
            float reduceDamage = 0;

            foreach (SupportSkillData passive in supportSkillList)
            {
                if (passive.SupportSkillName == SupportSkillName.Resurrection)
                    continue;
                moveRate += passive.MoveSpeedRate;
                atkRate += passive.AtkRate;
                criRate += passive.CriRate;
                cridmg += passive.CriDmg;
                reduceDamage += passive.DamageReduction;
            }
            
            MoveSpeedRate += moveRate;
            AttackBonusRate += atkRate;
            CriRate += criRate;
            CriticalDamage += cridmg;
            DamageReduction += reduceDamage;
        }

        public void OnUpgradeStatByMonsterKill()
        {
            List<SupportSkillData> supportSkillList = _skillBook.GetMonsterKillSupportSkillDataList();
            if (supportSkillList.Count == 0)
            {
                return;
            }
            
            float atkRate = 0;
            float reduceDamage = 0;
            float healRate = 0;

            foreach (SupportSkillData passive in supportSkillList)
            {
                if (passive.SupportSkillName == SupportSkillName.Resurrection)
                    continue;

                healRate += passive.HealRate;
                atkRate += passive.AtkRate;
                reduceDamage += passive.DamageReduction;
            }
            
            Heal(healRate);
            AttackBonusRate += atkRate;
            DamageReduction += reduceDamage;
        }

        public void OnKillEliteMonster()
        {
            List<SupportSkillData> supportSkillList = _skillBook.GetMonsterEliteSupportSkillDataList();
            if (supportSkillList.Count == 0)
            {
                return;
            }
            
            float expRate = 0;
            int soulCount = 0;
            foreach (SupportSkillData skillData in supportSkillList)
            {
                expRate += skillData.ExpBonusRate;
                soulCount += skillData.SoulAmount;
            }

            SoulAmount += soulCount;
            EXPBonusRate += expRate;
        }

        public void Heal(float healAmount)
        {
            float hp = MaxHP * healAmount;
            HP += hp;
        }

      
        public override Vector3 GetDirection()
        {
            return (_indicatorSpriteTransform.position - transform.position).normalized;
        }

        public List<TotalDamageInfoData> GetTotalDamageInfoData()
        {
            float totalDamage = GetTotalDamage();
            List<TotalDamageInfoData> list = new List<TotalDamageInfoData>();
            foreach (BaseSkill skill in _skillBook.ActivateSkillList)
            {
                var infoData = new TotalDamageInfoData
                {
                    skillSprite = Managers.Manager.I.Resource.Load<Sprite>(skill.SkillData.IconLabel),
                    skillName = skill.SkillData.Name,
                    skillDamageRatioByTotalDamage = skill.AccumulatedDamage / totalDamage,
                    skillAccumlatedDamage = skill.AccumulatedDamage
                };
                
                list.Add(infoData);
            }

            return list.OrderBy((a) => a.skillAccumlatedDamage).ToList();
        }

        private float GetTotalDamage()
        {
            float damage = 0;
            foreach (BaseSkill skill in _skillBook.ActivateSkillList)
            {
                damage += skill.AccumulatedDamage;
            }

            return damage;
        }
    }
}