using SlimeMaster.Interface;
using UniRx;

namespace SlimeMaster.Model
{
    public class PlayerModel : IModel
    {
        public ReactiveProperty<float> CurrentExpRatio = new();
        public ReactiveProperty<int> CurrentLevel = new();
        public ReactiveProperty<int> SoulAmount = new();
        public ReactiveProperty<int> CurrentExp = new();

        public void Reset()
        {
            CurrentExpRatio.Value = 0;
            CurrentLevel.Value = 1;
            SoulAmount.Value = 0;
            CurrentExpRatio.Value = 0;
        }
    }
}