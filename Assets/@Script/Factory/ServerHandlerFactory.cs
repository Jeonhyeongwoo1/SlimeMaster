using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SlimeMaster.Attribute;
using SlimeMaster.Firebase;
using SlimeMaster.Interface;
using SlimeMaster.Manager;
using SlimeMaster.Server;
using UnityEngine;

namespace SlimeMaster.Factory
{
    public class ServerHandlerFactory
    {
        private static readonly Dictionary<Type, IClientSender> ServerRequestHandlerDict = new();

        public static T Create<T>(params object[] injectParams) where T : IClientSender
        {
            if (!ServerRequestHandlerDict.TryGetValue(typeof(T), out var handler))
            {
                var interfaces = typeof(T).GetInterfaces()
                    .Where(x => x.GetCustomAttribute<ClientSenderAttribute>() != null);
                handler = (T)Activator.CreateInstance(typeof(T), injectParams);
                foreach (var @interface in interfaces)
                {
                    ServerRequestHandlerDict.Add(@interface, handler);
                }
            }

            return (T)handler;
        }

        public static T Get<T>() where T : class, IClientSender
        {
            if (ServerRequestHandlerDict.TryGetValue(typeof(T), out var handler))
            {
                return (T)handler;
            }

            Debug.LogError($"Failed Get {typeof(T)}");
            return null;
        }

        public static void ClearDict()
        {
            ServerRequestHandlerDict.Clear();
        }

        public static void InitializeServerHandlerRequest(FirebaseController firebaseController, DataManager dataManager)
        {
            ClearDict();
            Create<ServerUserRequestHandler>(firebaseController, dataManager);
            Create<ServerEquipmentRequestHandler>(firebaseController, dataManager);
            Create<ServerShopRequestHandler>(firebaseController, dataManager);
            Create<ServerCheckoutRequestHandler>(firebaseController, dataManager);
        }
    }
}