using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Interface;
using UniRx;
using UnityEngine;

namespace SlimeMaster.Model
{
    public class StageModel : IModel
    {
        public ReactiveProperty<int> timer = new();
        public ReactiveProperty<int> currentWaveStep = new();
        public ReactiveProperty<int> killCount = new();
    }
}