using System;
using UnityEngine;

namespace SlimeMaster.InGame.Input
{
    public static class InputHandler
    {
        public static Action<Vector2> onPointerDownAction;
        public static Action onPointerUpAction;
        public static Action<bool> onActivateInputHandlerAction;
    }
}