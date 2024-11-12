using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Input;
using SlimeMaster.InGame.Manager;
using SlimeMaster.InGame.Popup;
using SlimeMaster.InGame.Skill;
using SlimeMaster.InGame.View;
using SlimeMaster.Model;
using Unity.Mathematics;
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

        private PlayerStat _playerStat;

        [SerializeField] private PlayerStat _playerStatForView;

        #endregion
        
        public int Layer => _layer;

        public float CurrentExp
        {
            get => _currentExp;
            set
            {
                _currentExp += Mathf.RoundToInt(value * EXPBonusRate);
                var levelData = GameManager.I.Data.LevelDataDict[_playerModel.CurrentLevel.Value];
                _playerModel.CurrentExpRatio.Value = _currentExp / levelData.TotalExp;
                if (_currentExp >= levelData.TotalExp)
                {
                    GameManager.I.Event.Raise(GameEventType.LevelUp, _skillBook);
                    _currentExp -= levelData.TotalExp;
                }
            }
        }

        public int SoulAmount
        {
            get => _soulAmount;
            set
            {
                _soulAmount = value;
                _playerModel.SoulAmount.Value = _soulAmount;
            }
        }

        public override float AttackDamage => _creatureData.Atk * _creatureData.AtkRate * AttackBonusRate;
        
        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private Transform _indicatorSpriteTransform;
        
        private int _layer;
        private Vector2 _inputVector;
        private bool _isInit;
        private float _currentExp;
        private int _soulAmount;
        private PlayerModel _playerModel;

        private CancellationTokenSource _recoveryHPCts;

        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            _playerStat = GameManager.I.GameContinueData.playerStat;
            base.Initialize(creatureData, sprite, skillDataList);
            _isInit = true;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            _playerModel.CurrentLevel.Value = 1;
            _creatureType = CreatureType.Player;
            _moveSpeed = creatureData.MoveSpeed * creatureData.MoveSpeedRate;

            InitCreatureStat();
            
            //Default -> 추후에 장비에 맞춰서 변경되어야함.
            SkillData skillData = skillDataList.Find(v => v.DataId == (int)SkillType.StormBlade);
            _skillBook.UpgradeOrAddSkill(skillData);
            
            GameManager.I.CurrentSupportSkillDataList = _skillBook.GetRecommendSupportSkillDataList();
            List<BaseSkill> skillList = _skillBook.ActivateSkillList;
            UIManager uiManager = GameManager.I.UI;
            var gamesceneUI = uiManager.SceneUI as UI_GameScene;
            gamesceneUI.UpdateSkillSlotItem(skillList);
            gameObject.SetActive(true);
        }

        protected override void InitCreatureStat(bool isFullHP = true)
        {
            MaxHP = _creatureData.MaxHp;
            if (isFullHP)
            {
                HP = MaxHP;
            }

            // HP = MaxHP = 10000;
            // SoulAmount = 1000;
        }

        private void Start()
        {
            _layer = gameObject.layer;
            TryGetComponent(out _rigidbody);
            RecoveryHpAsync().Forget();
        }

        private void OnDestroy()
        {
            Utils.SafeCancelCancellationTokenSource(ref _recoveryHPCts);
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
        }
        
        private void OnEnable()
        {
            InputHandler.onPointerDownAction += OnChangedInputVector;
            InputHandler.onPointerUpAction += () => OnChangedInputVector(Vector2.zero);
            GameManager.I.Event.AddEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
        }

        public bool TryPurchaseSupportSkill(int supportSkillId)
        {
            SupportSkillData skillData = GameManager.I.Data.SupportSkillDataDict[supportSkillId];
            if (SoulAmount < skillData.Price)
            {
                Debug.Log(
                    $"failed purchase skill soul amount {SoulAmount} / price {skillData.Price} / skill Id {supportSkillId}");
                return false;
            }

            SoulAmount -= (int) skillData.Price;
            Debug.Log($"{SoulAmount}/ {skillData.Price}");
            _skillBook.AddSupportSkill(skillData);
            var lockSupportSkillList = GameManager.I.lockSupportSkillDataList;
            if (lockSupportSkillList.Contains(skillData))
            {
                lockSupportSkillList.Remove(skillData);
            }

            if (skillData.SupportSkillName == SupportSkillName.Healing)
            {
                Heal(skillData.HealRate);
                GameManager.I.Event.Raise(GameEventType.PurchaseSupportSkill, _skillBook.ActivateSupportSkillDataList);
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

            GameManager.I.Event.Raise(GameEventType.PurchaseSupportSkill, _skillBook.ActivateSupportSkillDataList);
            
            //Add effect

            return true;
        }

        private void UpdatePlayerStat(bool isFullHP = true)
        {
            InitCreatureStat(isFullHP);

            MaxHP *= MaxHPBonus;
            HP *= MaxHPBonus;
            _moveSpeed *= MoveSpeedRate;
            
            Debug.Log($"HP {HP} / MAXHP {MaxHP} / {MaxHPBonus}");
        }
        
        private void OnDisable()
        {
            InputHandler.onPointerDownAction -= OnChangedInputVector;
            InputHandler.onPointerUpAction -= () => OnChangedInputVector(Vector2.zero);

            if (GameManager.I)
            {
                GameManager.I.Event.RemoveEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
            }
        }
        
        private void FixedUpdate()
        {
            if (!_isInit || IsDeadState)
            {
                return;
            }
            
            Move();
            Rotate();
        }

        private void Update()
        {
            if (!_isInit || IsDeadState)
            {
                return;
            }

            GetDropItem();

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                SkillData skillData = _skillBook.ActivateSkillList
                    .Find(v => v.SkillType == SkillType.StormBlade).SkillData;
                _skillBook.UpgradeOrAddSkill(skillData);
            }
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
            }
        }
        
        private void GetDropItem()
        {
            GridController grid = GameManager.I.Stage.CurrentMap.Grid;
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

                        var gameSceneUI = GameManager.I.UI.SceneUI as UI_GameScene;
                        Transform tr = gameSceneUI.GetSoulIconTransform();
                        item.GetItem(tr, callback);
                        break;
                }
                
                GameManager.I.Object.DroppedItemControllerList.Remove(item);
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
            UpdateCreatureState(CreatureStateType.Idle);
            HP = 0;
            transform.DOKill();
            transform.DOScale(Vector3.zero, 0.3f);
            GameManager.I.Event.Raise(GameEventType.DeadPlayer);
            _skillBook?.StopAllSkillLogic();

            SupportSkill supportSkill = null;
            bool isPossibleResurrection = _skillBook.TryResurrection(ref supportSkill);
            if (isPossibleResurrection)
            {
                await DoResurrectionAsync(supportSkill);
            }
        }

        private async UniTask DoResurrectionAsync(SupportSkill supportSkill)
        {
            Heal(supportSkill.SupportSkillData.HealRate);
            MoveSpeedRate += supportSkill.SupportSkillData.MoveSpeedRate;
            AttackBonusRate += supportSkill.SupportSkillData.AtkRate;
            UpdatePlayerStat(false);
            
            _skillBook.UsedResurrectionSupportSkill(supportSkill);

            await UniTask.WaitForSeconds(1f, cancelImmediately: true);
            var prefab = GameManager.I.Resource.Instantiate(Const.Revival, transform);
            prefab.SetActive(true);
            prefab.transform.position = transform.position;
            DOVirtual.DelayedCall(1, () => GameManager.I.Pool.ReleaseObject(Const.Revival, prefab));
            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            UpdateCreatureState(CreatureStateType.Idle);
            _skillBook.UseAllSkillList(true, false, null);
            GameManager.I.Event.Raise(GameEventType.ResurrectionPlayer);
        }

        public void UpgradeOrAddSKill(SkillData skillData)
        {
            _skillBook.UpgradeOrAddSkill(skillData);
        }

        public int GetActivateSkillLevel(int skillId)
        {
            return _skillBook.GetActivateSkillLevel(skillId);
        }
        
        public void LevelUp()
        {
            _playerModel.CurrentLevel.Value++;
            CurrentExp = _currentExp;

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

        private void Move()
        {
            Vector3 position = transform.position;
            Vector3 prevPosition = position;
            Vector3 nextPosition = position + (Vector3)_inputVector;
            Vector3 lerp = Vector3.Lerp(prevPosition, nextPosition,
                Time.fixedDeltaTime * _moveSpeed);
            _rigidbody.MovePosition(lerp);
        }

        private void Rotate()
        {
            if (_inputVector == Vector2.zero)
            {
                return;
            }
            
            float angle = Mathf.Atan2(_inputVector.y, _inputVector.x) * Mathf.Rad2Deg - 90;
            _indicatorTransform.rotation = Quaternion.Euler(0, 0, angle);
            _spriteRenderer.flipX = math.sign(_inputVector.x) == 1;
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
                    skillSprite = GameManager.I.Resource.Load<Sprite>(skill.SkillData.IconLabel),
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