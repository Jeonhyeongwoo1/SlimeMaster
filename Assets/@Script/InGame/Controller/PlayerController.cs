using System;
using DG.Tweening;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Input;
using SlimeMaster.InGame.Manager;
using SlimeMaster.InGame.Skill;
using Unity.Mathematics;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class PlayerController : MonoBehaviour, IHitable
    {
        public int Layer => _layer;

        public Action<int, int> onHitReceived { get; set; }

        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private SpriteRenderer _characterSprite;
        [SerializeField] private float _moveSpeed = 10;
        
        private int _maxHp;
        private int _currentHp;
        private int _layer;
        private bool _isDead;
        private Vector2 _inputVector;
        private Rigidbody2D _rigidbody;

        private SkillBook _skillBook;

        public void Initialized()
        {
            _maxHp = _currentHp = 100;

            _skillBook = new SkillBook();
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
        }

        private void OnDisable()
        {
            InputHandler.onPointerDownAction -= OnChangedInputVector;
            InputHandler.onPointerUpAction -= () => OnChangedInputVector(Vector2.zero);
        }
        
        private void FixedUpdate()
        {
            Move();
            Rotate();
        }

        public void TakeDamage(float damage)
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

            onHitReceived?.Invoke(_currentHp, _maxHp);
        }

        private void Dead()
        {
            _isDead = true;
            _currentHp = 0;
            transform.DOScale(Vector3.zero, 0.3f);
            GameManager.I.Event.Raise(GameEventType.GameOver);
        }
        
        private void Move()
        {
            Vector3 position = transform.position;
            Vector3 prevPosition = position;
            Vector3 nextPosition = position + (Vector3)_inputVector;
            Vector3 lerp = Vector3.Lerp(prevPosition, nextPosition, Time.fixedDeltaTime * _moveSpeed);
            _rigidbody.MovePosition(lerp);
        }

        private void Rotate()
        {
            float angle = Mathf.Atan2(_inputVector.y, _inputVector.x) * Mathf.Rad2Deg - 90;
            _indicatorTransform.rotation = Quaternion.Euler(0, 0, angle);
            _characterSprite.flipX = math.sign(_inputVector.x) == 1;
        }
    }
}