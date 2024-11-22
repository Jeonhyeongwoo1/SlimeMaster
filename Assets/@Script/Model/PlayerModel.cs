using SlimeMaster.Interface;
using UniRx;

namespace SlimeMaster.Model
{
    public class PlayerModel : IModel
    {
        public ReactiveProperty<float> CurrentExpRatio = new();
        public ReactiveProperty<int> CurrentLevel = new();
        public ReactiveProperty<int> SoulAmount = new();
    }
}