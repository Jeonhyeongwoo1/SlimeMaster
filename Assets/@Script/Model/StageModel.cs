using SlimeMaster.Interface;
using UniRx;

namespace SlimeMaster.Model
{
    public class StageModel : IModel
    {
        public ReactiveProperty<int> timer = new();
        public ReactiveProperty<int> currentWaveStep = new();
        public ReactiveProperty<int> killCount = new();
    }
}