using da.Objects;
using da.Scenes;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    [GlobalClass]
    public partial class GameGlobal : Node, IEvent
    {
        #region 常量
        public const float MAX_FALL_SPEED = 400;
        public const string SAVE_PATH = "user://saves/save1.sav";
        public const string CONFIG_PATH = "user://config.ini";
        #endregion

        [Export] public UIControl UIControl;
        [Export] public Control LeftUpperBox;
        [Export] public TextureProgressBar healthBar;
        [Export] public ColorRect BlackMask;
        public Player player;
        public Camera2D camera;
        public SaveData save = new();
        public RootScene root;
        public bool isRunning = false;
        public bool IsChangingScene = false;

        [Signal] public delegate void CameraShakeEventHandler(float strength);
        public static GameGlobal Instance { get; private set; }
        public override void _Ready()
        {
            Instance = this;
            EventMgr.RegisterEvent(this);
            BlackMask.Modulate = new(BlackMask.Modulate, 0);
            BlackMask.MouseFilter = Control.MouseFilterEnum.Ignore;
            LoadConfig();
        }

        public async void Teleport(string sceneUrl, string positionName = null, int Direction = 1)
        {
            IsChangingScene = true;
            if (player != null && IsInstanceValid(player))
            {
                save.PlayerData = player.ToDict();
            }
            LeftUpperBox.Visible = false;
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
                GetTree().Paused = false;
                if (res != null)
                {
                    var mapnode = res.Instantiate<Control>();
                    root.WorldControl.AddChild(mapnode);
                    //await ToSignal(mapnode, Map.SignalName.Ready);
                    LoadMapData(mapnode);
                    if (positionName != null)
                    {
                        var arr = GetTree().GetNodesInGroup("Telepositions").Cast<Marker2D>();
                        foreach (Marker2D node in arr)
                        {
                            if (node.Name == positionName)
                            {
                                RootScene.TeleportPlayer(node.GlobalPosition, Direction);
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
                            RootScene.TeleportPlayer(node.GlobalPosition, Direction);
                            break;
                        }
                    }
                }
            }
            GetTree().Paused = false;
            await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
            var tween2 = CreateTween();
            tween2.SetPauseMode(Tween.TweenPauseMode.Process);
            tween2.TweenProperty(BlackMask, "modulate", new Color(BlackMask.Modulate, 0), 1f);
            tween2.Finished += () =>
            {
                BlackMask.MouseFilter = Control.MouseFilterEnum.Ignore;
                IsChangingScene = false;
                LeftUpperBox.Visible = true;
            };
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
                    //Player player = (Player)datas[0];
                    UpdateLeftUpperBox();
                    break;
                case "PlayerMaxHealthChanged":
                    healthBar.MaxValue = (double)datas[0];
                    break;
                case "RootReady":
                    root = (RootScene)datas[0];
                    break;
                case "EnemyDied":
                    RecordEnemyDied(datas[0] as Enemy);
                    break;
            }
        }

        public void UpdateLeftUpperBox()
        {
            healthBar.MaxValue = player?.status?.MaxHealth ?? 1;
            healthBar.Value = player?.status?.Health ?? 1;
            healthBar.MinValue = 0;
        }

        public void RecordEnemyDied(Enemy enemy)
        {
            var EnemyName = enemy.Name;
            var map = enemy.FindParent("*Map*") ?? enemy.FindParent("*map*");
            if (map != null)
            {
                if (!save.MapSaveData.ContainsKey(map.Name))
                {
                    save.MapSaveData[map.Name] = new Godot.Collections.Dictionary<string, Variant>();
                }
                if (!save.MapSaveData[map.Name].ContainsKey("enemies_died"))
                {
                    save.MapSaveData[map.Name].Add("enemies_died", new Godot.Collections.Array<string>());
                }
                Array<string> enemies_died = save.MapSaveData[map.Name]["enemies_died"].AsGodotArray<string>();
                enemies_died.Add(EnemyName);
                if (enemy.CantRevive)
                {
                    if (!save.MapSaveData[map.Name].ContainsKey("enemies_cantrevive"))
                    {
                        save.MapSaveData[map.Name].Add("enemies_cantrevive", new Godot.Collections.Array<string>());
                    }
                    Array<string> enemies_cantrevive = save.MapSaveData[map.Name]["enemies_cantrevive"].AsGodotArray<string>();
                    enemies_cantrevive.Add(EnemyName);
                }
                if (!save.GlobalSaveData.ContainsKey("enemy_kill_count")) save.GlobalSaveData.Add("enemy_kill_count", 1);
                else
                {
                    save.GlobalSaveData["enemy_kill_count"] = save.GlobalSaveData["enemy_kill_count"].AsInt32() + 1;
                }
            }
        }

        private void SaveMapData(Control map)
        {
            save.currMapPath = map.SceneFilePath.SimplifyPath();
            //GD.Print(save.currMapPath);
            //Godot.Collections.Array<string> enemies_died = new();
            //foreach (Enemy enemy in GetTree().GetNodesInGroup("enemies").Cast<Enemy>())
            //{
            //    enemies_alive.Add(GetPathTo(enemy));
            //}
            //if (!save.MapSaveData.ContainsKey(map.Name)) save.MapSaveData[map.Name] = new Godot.Collections.Dictionary<string, Variant>();
            //if (save.MapSaveData[map.Name].ContainsKey("enemies_alive")) save.MapSaveData[map.Name]["enemies_alive"] = enemies_alive;
            //else save.MapSaveData[map.Name].Add("enemies_alive", enemies_alive);
        }

        private void LoadMapData(Control map)
        {
            if (save.MapSaveData.ContainsKey(map.Name))
            {
                Array<string> enemies_died = null;
                if (save.MapSaveData[map.Name].ContainsKey("enemies_died"))
                {
                    enemies_died = save.MapSaveData[map.Name]["enemies_died"].AsGodotArray<string>();
                }
                Array<string> enemies_cantrevive = null;
                if (save.MapSaveData[map.Name].ContainsKey("enemies_cantrevive"))
                {
                    enemies_cantrevive = save.MapSaveData[map.Name]["enemies_cantrevive"].AsGodotArray<string>();
                }
                foreach (Enemy enemy in GetTree().GetNodesInGroup("enemies").Cast<Enemy>())
                {
                    if ((enemies_died != null && enemies_died.Contains(enemy.Name)) || (enemies_cantrevive != null && enemies_cantrevive.Contains(enemy.Name)))
                    {
                        enemy.QueueFree();
                    }
                }
            }
            foreach (SavePoint savePoint in GetTree().GetNodesInGroup("savepoints").Cast<SavePoint>())
            {
                if (save.ActivedSavePoints.ContainsKey(savePoint.Name))
                {
                    savePoint.Active = true;
                }
            }
            foreach (Door door in GetTree().GetNodesInGroup("doors").Cast<Door>())
            {
                if (save.MapSaveData.ContainsKey(map.Name) && save.MapSaveData[map.Name].ContainsKey(door.Name))
                {
                    door.IsOpened = save.MapSaveData[map.Name][door.Name].AsBool();
                }
            }
        }

        public void SaveGame()
        {
            save.currMapPath = (root.WorldControl.GetChild(0) as Control).SceneFilePath.GetFile().GetBaseDir();
            SaveMapData(root.WorldControl.GetChild(0) as Control);
            save.PlayerData = player.ToDict();
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
            AddMsg("保存游戏成功");
        }

        public async void LoadGame()
        {
            isRunning = true;
            using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
            if (file == null) return;
            var json = file.GetAsText();
            var data = Json.ParseString(json).AsGodotDictionary();
            save = new()
            {
                currMapPath = data["currMapPath"].AsString(),
                PlayerData = data["PlayerData"].AsGodotDictionary<string, Variant>(),
                MapSaveData = data["MapSaveData"].AsGodotDictionary<string, Godot.Collections.Dictionary<string, Variant>>(),
                ActivedSavePoints = data["ActivedSavePoints"].AsGodotDictionary<string, string>(),
                GlobalSaveData = data["GlobalSaveData"].AsGodotDictionary<string, Variant>(),
            };
            //save = Json.ParseString(json).As<SaveData>();
            await ChangeSceneAsync(save.currMapPath, true);

            //player.FromDict(save.PlayerData);
        }

        public static bool SaveFileExists()
        {
            using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
            return file != null;
        }

        public async Task ChangeSceneAsync(string path, bool loadPlayerData = false)
        {
            IsChangingScene = true;
            if (player != null && IsInstanceValid(player))
            {
                save.PlayerData = player.ToDict();
            }
            //UIControl.Visible = false;
            LeftUpperBox.Visible = true;
            GetTree().Paused = true;
            BlackMask.MouseFilter = Control.MouseFilterEnum.Stop;
            Tween tween = CreateTween();
            tween.SetPauseMode(Tween.TweenPauseMode.Process);
            tween.TweenProperty(BlackMask, "modulate", new Color(BlackMask.Modulate, 1), 1f);
            await ToSignal(tween, Tween.SignalName.Finished);
            if (root.WorldControl.GetChild(0) != null)
            {
                var mapName = (root.WorldControl.GetChild(0) as Control).Name;
                if (save.MapSaveData.ContainsKey(mapName))
                {
                    save.MapSaveData[mapName].Remove("enemies_died");
                }
                SaveMapData(root.WorldControl.GetChild(0) as Control);
            }
            if (path != root.WorldControl.GetChild(0).SceneFilePath.SimplifyPath())
            {
                foreach (var child in root.WorldControl.GetChildren())
                {
                    child.QueueFree();
                }
                PackedScene res = ResourceLoader.Load<PackedScene>(path);
                GetTree().Paused = false;
                if (res != null)
                {
                    var mapnode = res.Instantiate<Control>();
                    root.WorldControl.AddChild(mapnode);
                    //await ToSignal(mapnode, Control.SignalName.Ready);
                    if (mapnode.Name != "TitleScene") LoadMapData(mapnode);
                }
            }
            if (loadPlayerData)
            {
                player.FromDict(save.PlayerData);
            }
            GetTree().Paused = false;
            await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
            var tween2 = CreateTween();
            tween2.SetPauseMode(Tween.TweenPauseMode.Process);
            tween2.TweenProperty(BlackMask, "modulate", new Color(BlackMask.Modulate, 0), 1f);
            tween2.Finished += () =>
            {
                BlackMask.MouseFilter = Control.MouseFilterEnum.Ignore;
                IsChangingScene = false;
                LeftUpperBox.Visible = true;
            };
        }

        public void SaveConfig()
        {
            ConfigFile config = new();
            config.SetValue("audio", "master", SoundManager.GetVolume((int)SoundManager.AudioBusEnum.Master));
            config.SetValue("audio", "sfx", SoundManager.GetVolume((int)SoundManager.AudioBusEnum.SFX));
            config.SetValue("audio", "bgm", SoundManager.GetVolume((int)SoundManager.AudioBusEnum.BGM));
            config.Save(CONFIG_PATH);
        }

        public void LoadConfig()
        {
            ConfigFile config = new();
            config.Load(CONFIG_PATH);

            SoundManager.SetVolume((int)SoundManager.AudioBusEnum.Master, config.GetValue("audio", "master", 0.5f).AsSingle());
            SoundManager.SetVolume((int)SoundManager.AudioBusEnum.SFX, config.GetValue("audio", "sfx", 1f).AsSingle());
            SoundManager.SetVolume((int)SoundManager.AudioBusEnum.BGM, config.GetValue("audio", "bgm", 1f).AsSingle());
        }

        public void ShakeCamera(float strength)
        {
            EmitSignal(SignalName.CameraShake, strength);
        }
    }
}
