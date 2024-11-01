using da.Scripts;
using da.Scripts.Objects;
using Godot;
using Godot.Collections;

namespace DialogicRuntime
{
    public partial class Dialogic : Node
    {
        public static Node Instance
        {
            get;
            private set;
        }

        public override void _Ready()
        {
            Instance = GetNode<Node>("/root/Dialogic");
            Prepare();
        }

        public enum States
        {
            IDLE,
            REVEALING_TEXT,
            ANIMATING,
            AWAITING_CHOICE,
            WAITING
        }

        public enum ClearFlags
        {
            FULL_CLEAR = 0,
            KEEP_VARIABLES = 1,
            TIMELINE_INFO_ONLY = 2
        }

        public static DialogicTimeline CurrentTimeline
        {
            get => (DialogicTimeline)Instance.Get("current_timeline");
            set => Instance.Set("current_timeline", value);
        }
        public static Array CurrentTimelineEvents
        {
            get => (Array)Instance.Get("current_timeline_events");
            set => Instance.Set("current_timeline_events", value);
        }
        public static int CurrentEventIdx
        {
            get => (int)Instance.Get("current_event_idx");
            set => Instance.Set("current_event_idx", value);
        }

        public static Dictionary CurrentStateInfo
        {
            get => (Dictionary)Instance.Get("current_state_info");
            set => Instance.Set("current_state_info", value);
        }

        public static States CurrentState
        {
            get => (States)Instance.Get("current_state").AsInt32();
            set => Instance.Set("current_state", (int)value);
        }
        public static bool Paused
        {
            get => (bool)Instance.Get("paused");
            set => Instance.Set("paused", value);
        }

        public delegate void StateChangedEventHandler(States NewState);
        public delegate void DialogicPausedEventHandler();
        public delegate void DialogicResumedEventHandler();
        public delegate void TimelineEndedEventHandler();
        public delegate void TimelineStartedEventHandler();
        //public delegate void EventHandledEventHandler(DialogicEvent resource);
        public delegate void SignalEventEventHandler(Variant argument);
        public delegate void TextSignalEventHandler(string argument);

        public static StateChangedEventHandler StateChanged;
        public static DialogicPausedEventHandler DialogicPaused;
        public static DialogicResumedEventHandler DialogicResumed;
        public static TimelineEndedEventHandler TimelineEnded;
        public static TimelineStartedEventHandler TimelineStarted;
        //public static EventHandledEventHandler EventHandled;
        public static SignalEventEventHandler SignalEvent;
        public static TextSignalEventHandler TextSignal;

        public static Node Start(string timeline, string label = "")
        {
            if (GameGlobal.Instance.player != null) GameGlobal.Instance.player.StateMachine.ChangeState(da.Scripts.Objects.PlayerScript.PlayerState.Idle);
            return (Node)Instance.Call("start", timeline, label);
        }

        public static void StartTimeline(string timeline, string label_or_idx = "")
        {
            Instance.Call("start_timeline", timeline, label_or_idx);
        }

        public static void StartTimeline(string timeline, int label_or_idx)
        {
            Instance.Call("start_timeline", timeline, label_or_idx);
        }

        public static void Prepare()
        {
            Instance.Connect("state_changed", Callable.From((States newState) => StateChanged?.Invoke(newState)));
            Instance.Connect("dialogic_paused", Callable.From(() => DialogicPaused?.Invoke()));
            Instance.Connect("dialogic_resumed", Callable.From(() => DialogicResumed?.Invoke()));
            Instance.Connect("timeline_ended", Callable.From(() => TimelineEnded?.Invoke()));
            Instance.Connect("timeline_started", Callable.From(() => TimelineStarted?.Invoke()));
            //Instance.Connect("event_handled", Callable.From((DialogicEvent resource) => EventHandled?.Invoke(resource)));
            Instance.Connect("signal_event", Callable.From((Variant argument) => SignalEvent?.Invoke(argument)));
            Instance.Connect("text_signal", Callable.From((string argument) => TextSignal?.Invoke(argument)));

            SignalEvent += (Variant eventDic) =>
            {
                var dic = eventDic.AsGodotDictionary();
                if (dic.ContainsKey("eventName"))
                {
                    string eventName = dic["eventName"].AsString();
                    Variant? a1 = null;
                    if (dic.ContainsKey("Argument1")) a1 = dic["Argument1"];
                    Variant? a2 = null;
                    if (dic.ContainsKey("Argument2")) a2 = dic["Argument2"];
                    Variant? a3 = null;
                    if (dic.ContainsKey("Argument3")) a3 = dic["Argument3"];
                    switch (eventName)
                    {
                        case "Teleport":
                            GameGlobal.Instance.Teleport(dic["Argument1"].AsString(), a2?.AsString(), a3?.AsInt32() ?? 1);
                            break;
                        case "CameraShake":
                            GameGlobal.Instance.ShakeCamera(a1?.AsSingle() ?? 0f);
                            break;
                    }
                }
            };
        }

        public static void send_update(string eventName, params object[] args)
        {
            EventMgr.DispatchEvent(eventName, args);
        }
    }

    public partial class DialogicTimeline : RefCounted
    {
        private Array<string> events = new();
        public Array<string> Events
        {
            get => events;
            set => events = value;
        }

        private bool EventsProcessed = false;
        public bool EventsProcessed_ { get => EventsProcessed; set => EventsProcessed = value; }

        public DialogicTimeline(RefCounted data)
        {
            Events = (Array<string>)data.Get("events");
            EventsProcessed = (bool)data.Get("events_processed");
        }
    }

    //public partial class DialogicEvent : RefCounted
    //{
    //    public delegate void EventStartedEventHandler(DialogicEvent resource);
    //    public delegate void EventFinishedEventHandler(DialogicEvent resource);
    //}
}
