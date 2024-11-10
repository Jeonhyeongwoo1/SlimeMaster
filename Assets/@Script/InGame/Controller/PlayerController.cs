using System;
using System.Collections.Generic;
using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Input;
using SlimeMaster.InGame.Manager;
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
        
        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private Transform _indicatorSpriteTransform;
        
        private int _layer;
        private bool _isDead;
        private Vector2 _inputVector;
        private bool _isInit;
        private int _currentExp;
        private PlayerModel _playerModel;

        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);
            _isInit = true;
            _isDead = false;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            _playerModel.CurrentLevel.Value = 1;

            //Default -> 추후에 장비에 맞춰서 변경되어야함.
            SkillData skillData = skillDataList.Find(v => v.DataId == (int)SkillType.StormBlade);
            _skillBook.UpgradeOrAddSkill(skillData);
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
                        break;
                    case DropableItemType.Potion:
                    case DropableItemType.Magnet:
                    case DropableItemType.Bomb:
                        break;
                }

                item.GetItem(transform, callback);
            }
        }

        public override void TakeDamage(float damage)
        {
            if (_isDead)
            {
                return;
            }

            _currentHp -= (int)damage;

            Debug.Log(_currentHp);
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
    }
}