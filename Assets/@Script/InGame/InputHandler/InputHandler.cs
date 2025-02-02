using System;
using UnityEngine;

namespace SlimeMaster.InGame.Input
{
    public static class InputHandler
    {
        public static Action<Vector2> onInputAction;
        public static Action<Vector2> onPointerUpAction;
        public static Action<bool> onActivateInputHandlerAction;
    }
}