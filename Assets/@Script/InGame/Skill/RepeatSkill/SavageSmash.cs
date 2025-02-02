using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class SavageSmash : RepeatSkill
    {
        protected override string HitSoundName => "SavageSmash_Hit";

        private IGeneratable _generatable;
        
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        private void Release()
        {
            if (_generatable != null)
            {
                _generatable.Release();
                _generatable = null;
            }
        }

        protected override async UniTask UseSkill()
        {
            if (_skillLogicCts == null || _skillLogicCts.IsCancellationRequested)
            {
                return;
            }
            
            Release();
            string prefabLabel = "SavageSmash";
            GameObject prefab = Managers.Manager.I.Resource.Instantiate(prefabLabel);
            _generatable = prefab.GetComponent<IGeneratable>();
            _generatable.OnHit = OnHit;
            _generatable.Level = CurrentLevel;
            _generatable.Generate(_owner.Position, Vector3.zero, _skillData, _owner);

            try
            {
                await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
            }
        }

        public override void OnChangedSkillData()
        {
            _generatable?.OnChangedSkillData(_skillData);
        }
    }
}