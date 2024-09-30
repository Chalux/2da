using System.Collections.Generic;

namespace da.Scripts
{
    public static class EventMgr
    {
        public static readonly HashSet<IEvent> events = new();

        public static void RegisterEvent(IEvent e)
        {
            events.Add(e);
        }

        public static void UnRegisterEvent(IEvent e)
        {
            events.Remove(e);
        }

        public static void DispatchEvent(string eventName, params object[] datas)
        {
            foreach (var e in events)
            {
                e.ReceiveEvent(eventName, datas);
            }
        }
    }

    public interface IEvent
    {
        public abstract void ReceiveEvent(string eventName, params object[] datas);
    }
}
