using System;
using UnityEngine;

namespace GameLogic
{
    internal static class GameSaveConnections
    {
        private static IGameProgressSave _active;
        private static event Action<IGameProgressSave> Changed;

        public static void Connect(Action<IGameProgressSave> bind)
        {
            Changed += bind;
            bind(_active);
        }

        public static void Disconnect(Action<IGameProgressSave> bind)
        {
            Changed -= bind;
            bind(null);
        }

        public static void SetActive(IGameProgressSave save)
        {
            _active = save;
            Changed?.Invoke(save);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _active = null;
            Changed = null;
        }
    }
}
