using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using SlimeMaster.InGame.Skill;
using UnityEngine;

public class CreatureController : MonoBehaviour, IHitable
{
    public string PrefabLabel=> _creatureData.PrefabLabel;
    public Vector3 Position => transform.position;
    public virtual float AttackDamage => _attackDamage;
    public SkillBook SkillBook => _skillBook;
    public Rigidbody2D Rigidbody => _rigidbody;
    public CreatureData CreatureData => _creatureData;
    public float CircleColliderRadius => _collider ? _collider.radius : 0;

    public bool IsDeadState => _creatureStateType == CreatureStateType.Dead;
    public CreatureType CreatureType => _creatureType;
    
    public virtual float MaxHP { get; set; }
    public virtual float HP { get; set; }
    
    [SerializeField] protected CreatureStateType _creatureStateType;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Rigidbody2D _rigidbody;

    protected CreatureType _creatureType;
    protected SkillBook _skillBook;
    protected CreatureData _creatureData;
    protected CircleCollider2D _collider;
    protected float _attackDamage;
    protected float _moveSpeed;
    
    public virtual void TakeDamage(float damage, CreatureController attacker)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Dead();
        }
    }

    public virtual void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
    {
        _spriteRenderer.sprite = sprite;
        _creatureData = creatureData;
        _collider = GetComponentInChildren<CircleCollider2D>();
        InitCreatureStat();
        AddSkillBook(skillDataList);
        Reset();
    }

    private void Reset()
    {
        _rigidbody.simulated = true;
        _creatureStateType = CreatureStateType.Idle;
    }

    protected virtual void InitCreatureStat(bool isFullHP = true)
    {
        float waveRate = GameManager.I.Stage.WaveData.HpIncreaseRate;
        MaxHP = (CreatureData.MaxHp + (CreatureData.MaxHpBonus * GameManager.I.Stage.StageData.StageLevel)) *
                (CreatureData.HpRate + waveRate);
        _attackDamage = (CreatureData.Atk + (CreatureData.AtkBonus * GameManager.I.Stage.StageData.StageLevel)) *
                        CreatureData.AtkRate;
        HP = MaxHP;
        _moveSpeed = CreatureData.MoveSpeed * CreatureData.MoveSpeedRate;
    }

    private void AddSkillBook(List<SkillData> skillDataList)
    {
        _skillBook ??= new SkillBook(this, skillDataList);
    }

    protected virtual async void Dead()
    {
        _creatureStateType = CreatureStateType.Dead;
    }

    public Action<int, int> onHitReceived { get; set; }

    protected async UniTask DeadAnimation()
    {
        ResourcesManager resource = GameManager.I.Resource;
        Material defaultMat =  resource.Load<Material>("CreatureDefaultMat");
        Material hitEffectMat = resource.Load<Material>("PaintWhite");

        _rigidbody.simulated = false;
        _spriteRenderer.material = defaultMat;
        await UniTask.WaitForSeconds(0.1f);
        
        _spriteRenderer.material = hitEffectMat;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(0, 0.2f).SetEase(Ease.InOutBounce));
        sequence.OnComplete(() =>
        {
            _rigidbody.simulated = true;
            _spriteRenderer.material = defaultMat;
            transform.localScale = Vector3.one;
        });

        await UniTask.WaitForSeconds(0.2f);
    }

    public void SetSpriteFlipX(bool isRight)
    {
        _spriteRenderer.flipX = isRight;
    }

    public virtual Vector3 GetDirection()
    {
        return Vector3.zero;
    }
    
    public virtual void UpdateStateAndAnimation(CreatureStateType stateType, string animationName)
    {
    }

    public void UpdateCreatureState(CreatureStateType stateType)
    {
        _creatureStateType = stateType;
    }

    public void RigidBodyMovePosition(Vector2 direction)
    {
        _rigidbody.MovePosition(_rigidbody.position + direction);
        SetSpriteFlipX(direction.x > 0);
    }
}
