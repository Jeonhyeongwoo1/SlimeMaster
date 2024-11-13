using SlimeMaster.InGame.Interface;
using UniRx;

namespace SlimeMaster.Factory
{
    public class UserModel : IModel
    {
        public ReactiveProperty<long> Diamond = new();
        public ReactiveProperty<long> Gold = new();
        public ReactiveProperty<long> Stamina = new();
    }
}