using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class EletronicField : RepeatSkill
    {
        protected override string HitSoundName => "EletronicField_Hit";
        private IGeneratable _generatable;
        
        public override void StopSkillLogic()
        {
            Release();
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
            Release();
            GameObject prefab = Managers.Manager.I.Resource.Instantiate(_skillData.PrefabLabel);
            var shootable = prefab.GetComponent<IGeneratable>();
            shootable.OnHit = OnHit;
            shootable.Generate(_owner.Position, Vector3.zero, _skillData, _owner);

            await UniTask.WaitUntil(() => _skillLogicCts.IsCancellationRequested);
        }

        public override void OnChangedSkillData()
        {
            _generatable?.OnChangedSkillData(_skillData);
        }
    }
}