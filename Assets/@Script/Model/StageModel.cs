using SlimeMaster.Interface;
using UniRx;

namespace SlimeMaster.Model
{
    public class StageModel : IModel
    {
        public ReactiveProperty<int> timer = new();
        public ReactiveProperty<int> currentWaveStep = new();
        public ReactiveProperty<int> killCount = new();

        public void Reset()
        {
            timer.Value = 0;
            currentWaveStep.Value = 0;
            killCount.Value = 0;
        }
    }
}