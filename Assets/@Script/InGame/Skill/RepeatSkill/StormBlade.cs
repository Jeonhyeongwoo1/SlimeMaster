using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class StormBlade : RepeatSkill
    {
        private List<IGeneratable> _generatableList = new ();
        
        public override void StopSkillLogic()
        {
            Release();
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        private void Release()
        {
            if (_generatableList.Count > 0)
            {
                _generatableList.ForEach(v=> v?.Release());
                _generatableList.Clear();
            }
        }

        public override void OnChangedSkillData()
        {
            if (_generatableList.Count > 0)
            {
                foreach (IGeneratable generatable in _generatableList)
                {
                    generatable.OnChangedSkillData(_skillData);
                }
            }
        }

        protected override async UniTask UseSkill()
        {
            Release();
            int count = 7;
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(45 + 45 * i, Vector3.forward) * _owner.GetDirection();
                for (int j = 0; j < projectileCount; j++)
                {
                    float angle = SkillData.AngleBetweenProj * (j - (SkillData.NumProjectiles - 1) / 2f);
                    Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                    GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    generatable.Generate(_owner.Position, direction, _skillData, _owner);
                    _generatableList.Add(generatable);
                }
                
                try
                {
                    await UniTask.WaitForSeconds(_skillData.AttackInterval, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                    StopSkillLogic();
                    StartSkillLogicProcessAsync().Forget();
                }
            }
        }
    }
}