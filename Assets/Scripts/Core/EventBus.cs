using System;
using System.Collections.Generic;

namespace Farm
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> handler) where T : struct
        {
            Type t = typeof(T);
            _handlers.TryGetValue(t, out Delegate existing);
            _handlers[t] = Delegate.Combine(existing, handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            Type t = typeof(T);
            if (!_handlers.TryGetValue(t, out Delegate existing))
            {
                return;
            }

            Delegate updated = Delegate.Remove(existing, handler);
            if (updated == null)
            {
                _handlers.Remove(t);
            }
            else
            {
                _handlers[t] = updated;
            }
        }

        public static void Publish<T>(T evt) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out Delegate del))
            {
                (del as Action<T>)?.Invoke(evt);
            }
        }

        public static void Clear()
        {
            _handlers.Clear();
        }
    }
}
