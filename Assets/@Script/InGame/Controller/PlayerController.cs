using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Entity;
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
        public int Layer => _layer;

        public int CurrentExp
        {
            get => _currentExp;
            set
            {
                _currentExp = value;
                var levelData = GameManager.I.Data.LevelDataDict[_playerModel.CurrentLevel.Value];
                _playerModel.CurrentExpRatio.Value = (float)_currentExp / levelData.TotalExp;
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
        
        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private Transform _indicatorSpriteTransform;
        
        private int _layer;
        private bool _isDead;
        private Vector2 _inputVector;
        private bool _isInit;
        private int _currentExp;
        private int _soulAmount;
        private PlayerModel _playerModel;

        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);
            _isInit = true;
            _isDead = false;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            _playerModel.CurrentLevel.Value = 1;
            _creatureType = CreatureType.Player;

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

        private void Start()
        {
            _layer = gameObject.layer;
            TryGetComponent(out _rigidbody);
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
            
            _skillBook.AddSupportSkill(skillData);
            var lockSupportSkillList = GameManager.I.lockSupportSkillDataList;
            if (lockSupportSkillList.Contains(skillData))
            {
                lockSupportSkillList.Remove(skillData);
            }

            GameManager.I.Event.Raise(GameEventType.PurchaseSupportSkill, _skillBook.ActivateSupportSkillDataList);
            
            //Add effect

            return true;
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
            if (!_isInit || _isDead)
            {
                return;
            }
            
            Move();
            Rotate();
        }

        private void Update()
        {
            if (!_isInit || _isDead)
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
                        Heal(healAmount);
                    }
                    break;
                
            }
        }
        
        private void GetDropItem()
        {
            GridController grid = GameManager.I.Stage.CurrentMap.Grid;
            List<DropItemController> itemControllerList = grid.GetDropItem(transform.position);
            
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
                            CurrentExp += exp;
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
                            SoulAmount += soul.GetSoulAmount();
                        };

                        var gameSceneUI = GameManager.I.UI.SceneUI as UI_GameScene;
                        Transform tr = gameSceneUI.GetSoulIconTransform();
                        item.GetItem(tr, callback);
                        break;
                }

            }
        }

        public override void TakeDamage(float damage)
        {
            if (_isDead)
            {
                return;
            }

            _currentHp -= (int)damage;

            if (_currentHp <= 0)
            {
                Dead();
                _currentHp = 0;
                _isDead = true;
            }

            onHitReceived?.Invoke((int)_currentHp, (int)_creatureData.MaxHp);
        }

        protected override void Dead()
        {
            _isDead = true;
            _currentHp = 0;
            transform.DOScale(Vector3.zero, 0.3f);
            GameManager.I.Event.Raise(GameEventType.GameOver);
            _skillBook?.StopAllSkillLogic();
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
        }

        public void Heal(float healAmount)
        {
            Debug.Log("heal : " + healAmount);
            float hp = _creatureData.MaxHp * healAmount;
            _currentHp += hp;
        }
        
        private void Move()
        {
            Vector3 position = transform.position;
            Vector3 prevPosition = position;
            Vector3 nextPosition = position + (Vector3)_inputVector;
            Vector3 lerp = Vector3.Lerp(prevPosition, nextPosition,
                Time.fixedDeltaTime * _creatureData.MoveSpeed * _creatureData.MoveSpeedRate);
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