using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Objects
{
    public partial class Campfire : Control
    {
        [Export] public AnimatedSprite2D sprite;
        [Export] public SavePoint saveArea;
        [Export] public PointLight2D pointLight;
        [Export] public AudioStreamPlayer2D audioPlayer;
        [Export] public Timer FrameTimer;
        Random random = new Random();
        [Export] public Timer PointLightTimer;
        [Export] public string SavePointName = "campfire";

        public override void _Ready()
        {
            FrameTimer.Timeout += OnFrameTimeout;
            PointLightTimer.Timeout += OnPointLightTimeout;
            saveArea.Name = SavePointName;
        }

        private void OnFrameTimeout()
        {
            sprite.Frame = random.Next(0, 40);
        }

        private void OnPointLightTimeout()
        {
            pointLight.TextureScale = random.NextSingle() * 0.5f + 1f;
        }
    }
}
