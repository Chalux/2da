using da.Objects;
using da.Scenes;
using Godot;
using System.Linq;

namespace da.Scripts.Objects
{
    [GlobalClass]
    public partial class GameGlobal : Node, IEvent
    {
        #region 常量
        public const float MAX_FALL_SPEED = 400;
        public const string SAVE_PATH = "user://saves/save1.sav";
        #endregion

        [Export] public UIControl UIControl;
        [Export] public TextureProgressBar healthBar;
        [Export] public ColorRect BlackMask;
        public SaveData save = new();
        public RootScene root;
        public static GameGlobal Instance { get; private set; }
        public override void _Ready()
        {
            Instance = this;
            EventMgr.RegisterEvent(this);
            BlackMask.Modulate = new(BlackMask.Modulate, 0);
            BlackMask.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
        public void InSceneTeleport(string positionName, int Direction)
        {
            foreach (Marker2D node in GetTree().GetNodesInGroup("Telepositions").Cast<Marker2D>())
            {
                if (node.Name == positionName)
                {
                    (GetTree().CurrentScene as RootScene).TeleportPlayer(node.GlobalPosition, Direction);
                    break;
                }
            }
        }

        public async void Teleport(string sceneUrl, string positionName = null, int Direction = 1)
        {
            GetTree().Paused = true;
            BlackMask.MouseFilter = Control.MouseFilterEnum.Stop;
            Tween tween = CreateTween();
            tween.SetPauseMode(Tween.TweenPauseMode.Process);
            tween.TweenProperty(BlackMask, "modulate", new Color(BlackMask.Modulate, 1), 1f);
            await ToSignal(tween, Tween.SignalName.Finished);
            if (root.WorldControl.GetChild(0) != null) SaveMapData(root.WorldControl.GetChild(0) as Control);
            if (sceneUrl != root.WorldControl.GetChild(0).SceneFilePath.SimplifyPath())
            {
                foreach (var child in root.WorldControl.GetChildren())
                {
                    child.QueueFree();
                }
                PackedScene res = ResourceLoader.Load<PackedScene>(sceneUrl);
                var mapnode = res.Instantiate<Control>();
                if (res != null)
                {
                    root.WorldControl.AddChild(mapnode);
                    LoadMapData(mapnode);
                    if (positionName != null)
                    {
                        var arr = GetTree().GetNodesInGroup("Telepositions").Cast<Marker2D>();
                        foreach (Marker2D node in arr)
                        {
                            if (node.Name == positionName)
                            {
                                (GetTree().CurrentScene as RootScene).TeleportPlayer(node.GlobalPosition, Direction);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (positionName != null)
                {
                    var arr = GetTree().GetNodesInGroup("Telepositions").Cast<Marker2D>();
                    foreach (Marker2D node in arr)
                    {
                        if (node.Name == positionName)
                        {
                            (GetTree().CurrentScene as RootScene).TeleportPlayer(node.GlobalPosition, Direction);
                            break;
                        }
                    }
                }
            }
            var tween2 = CreateTween();
            tween2.SetPauseMode(Tween.TweenPauseMode.Process);
            tween2.TweenProperty(BlackMask, "modulate", new Color(BlackMask.Modulate, 0), 1f);
            await ToSignal(tween2, Tween.SignalName.Finished);
            BlackMask.MouseFilter = Control.MouseFilterEnum.Ignore;
            GetTree().Paused = false;
        }

        public void AddMsg(string msg)
        {
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Objects/MsgBox.tscn");
            if (res == null) return;
            MsgBox msgBox = res.Instantiate<MsgBox>();
            msgBox.SetMsg(msg);
            UIControl.MsgContainer.AddChild(msgBox);
        }

        public void PlayerHealthChanged(double newhealth, double oldhealth)
        {
            healthBar.Value = newhealth;
        }

        public void PlayerMaxHealthChanged(double newhealth)
        {
            healthBar.MaxValue = newhealth;
        }

        public void ReceiveEvent(string eventName, params object[] datas)
        {
            switch (eventName)
            {
                case "PlayerReady":
                    Player player = (Player)datas[0];
                    healthBar.MaxValue = player.status.MaxHealth;
                    healthBar.Value = player.status.Health;
                    healthBar.MinValue = 0;
                    break;
                case "PlayerMaxHealthChanged":
                    healthBar.MaxValue = (double)datas[0];
                    break;
                case "RootReady":
                    root = (RootScene)datas[0];
                    break;
            }
        }

        private void SaveMapData(Control map)
        {
            save.currMapPath = map.SceneFilePath.SimplifyPath();
            GD.Print(save.currMapPath);
            Godot.Collections.Array<string> enemies_alive = new();
            foreach (Enemy enemy in GetTree().GetNodesInGroup("enemies").Cast<Enemy>())
            {
                enemies_alive.Add(GetPathTo(enemy));
            }
            if (!save.MapSaveData.ContainsKey(map.Name)) save.MapSaveData[map.Name] = new Godot.Collections.Dictionary<string, Variant>();
            if (save.MapSaveData[map.Name].ContainsKey("enemies_alive")) save.MapSaveData[map.Name]["enemies_alive"] = enemies_alive;
            else save.MapSaveData[map.Name].Add("enemies_alive", enemies_alive);
        }

        private void LoadMapData(Control map)
        {
            if (save.MapSaveData.ContainsKey(map.Name) && save.MapSaveData[map.Name].ContainsKey("enemies_alive"))
            {
                Godot.Collections.Array<string> enemies_alive = save.MapSaveData[map.Name]["enemies_alive"].AsGodotArray<string>();
                foreach (Enemy enemy in GetTree().GetNodesInGroup("enemies").Cast<Enemy>())
                {
                    if (!enemies_alive.Contains(GetPathTo(enemy)))
                    {
                        enemy.QueueFree();
                    }
                }
            }
        }

        public void SaveGame()
        {
            save.currMapPath = (root.WorldControl.GetChild(0) as Control).SceneFilePath.GetFile().GetBaseDir();
            SaveMapData(root.WorldControl.GetChild(0) as Control);
            save.PlayerData = root.player.ToDict();
            var json = Json.Stringify(save.ToList());
            if (!DirAccess.DirExistsAbsolute("user://saves"))
            {
                DirAccess.MakeDirAbsolute("user://saves");
            }
            using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                GD.PrintErr(FileAccess.GetOpenError());
                return;
            }
            file.StoreString(json);
        }

        public void LoadGame()
        {
            using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
            if (file == null) return;
            var json = file.GetAsText();
            var data = Json.ParseString(json).AsGodotDictionary();
            save = new()
            {
                currMapPath = data["currMapPath"].AsString(),
                PlayerData = data["PlayerData"].AsGodotDictionary<string, Variant>(),
                MapSaveData = data["MapSaveData"].AsGodotDictionary<string, Godot.Collections.Dictionary<string, Variant>>(),
            };
            Teleport(save.currMapPath);

            root.player.FromDict(save.PlayerData);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                SaveGame();
            }
            else if (@event.IsActionPressed("ui_filedialog_up_one_level"))
            {
                LoadGame();
            }
        }
    }
}
