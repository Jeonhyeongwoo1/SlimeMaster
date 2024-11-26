using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ArrowShot : RepeatSkill
    {
        private List<IGeneratable> _generatableList;
        protected override string HitSoundName => "ArrowShot_Hit";

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        private void Release()
        {
            if (_generatableList == null)
            {
                _generatableList.ForEach(v=> v?.Release());
                _generatableList.Clear();
            }
        }
        
        public override void OnChangedSkillData()
        {
            _generatableList?.ForEach(v=> v.OnChangedSkillData(_skillData));
        }

        protected override async UniTask UseSkill()
        {
            int projectile = _skillData.NumProjectiles;
            for (int i = 0; i < projectile; i++)
            {
                GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.Generate(_owner.Position, _owner.GetDirection(), _skillData, _owner);
                _generatableList.Add(generatable);
                
                try
                {
                    await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancelImmediately: true);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                    StopSkillLogic();
                    break;
                }
            }
        }
    }
}