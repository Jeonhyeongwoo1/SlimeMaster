using System;
using System.Collections.Generic;
using SlimeMaster.Presenter;

namespace SlimeMaster.Factory
{
    public static class PresenterFactory
    {
        private static readonly Dictionary<Type, BasePresenter> _dict = new();

        public static T CreateOrGet<T>() where T : BasePresenter, new()
        {
            if (!_dict.TryGetValue(typeof(T), out var presenter))
            {
                presenter = new T();
                _dict.Add(typeof(T), presenter);
            }

            return (T)presenter;
        }

        public static void Clear()
        {
            _dict.Clear();;
        }
    }
}