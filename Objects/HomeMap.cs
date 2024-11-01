using da.Scripts;
using da.Scripts.Objects;
using DialogicRuntime;
using Godot;
using System.Collections.Generic;

namespace da.Objects
{
    public partial class HomeMap : Map
    {
        [Export] public Area2D InsideChecker;
        [Export] public SpeakPoint HomeSpeakPoint1;
        [Export] public GpuParticles2D WindParticles;
        [Export] public AudioStreamPlayer WindSound;
        bool _isInside = false;
        public bool IsInside
        {
            get => _isInside;
            set
            {
                _isInside = value;
                if (_isInside)
                {
                    WindParticles.Emitting = false;
                    WindSound.StreamPaused = true;
                }
                else
                {
                    WindParticles.Emitting = true;
                    WindSound.StreamPaused = false;
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();
            InsideChecker.BodyEntered += OutDoorBodyEntered;
            InsideChecker.BodyExited += OutDoorBodyExited;

            if (!GameGlobal.Instance.save.MapSaveData.ContainsKey(Name))
            {
                GameGlobal.Instance.save.MapSaveData.Add(Name, new Godot.Collections.Dictionary<string, Variant>());
            }
            bool hasSpeakDream = GameGlobal.Instance.save.MapSaveData[Name].GetValueOrDefault("hasSpeakDream", false).AsBool();
            if (!hasSpeakDream)
            {
                Dialogic.Start("HomeDream1");
                if (!GameGlobal.Instance.save.MapSaveData[Name].TryAdd("hasSpeakDream", true))
                {
                    GameGlobal.Instance.save.MapSaveData[Name]["hasSpeakDream"] = true;
                }
                player.status.Health = player.status.MaxHealth;
            }
        }

        private void OutDoorBodyEntered(Node2D body)
        {
            IsInside = true;
        }

        private void OutDoorBodyExited(Node2D body)
        {
            IsInside = false;
        }
    }
}
