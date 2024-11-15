using da.Objects;
using da.Scenes;
using Godot;
using Godot.Collections;
using System;
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
        [Export] public HBoxContainer SkillSlotContainer;
        public Player player;
        public Camera2D camera;
        public SaveData save = new();
        public RootScene root;
        public bool isRunning = false;
        public bool IsChangingScene = false;
        [Export] public Timer richHintTimer;
        [Export] public RichTextLabel richHint;
        [Export] public ColorRect richHintBg;
        [Export] public SkillBtn skillBtn1;
        [Export] public SkillBtn skillBtn2;
        [Export] public SkillBtn skillBtn3;
        [Export] public SkillBtn skillBtn4;
        [Export] public Label HealthLab;

        [Signal] public delegate void CameraShakeEventHandler(float strength);
        public static GameGlobal Instance { get; private set; }
        public override void _Ready()
        {
            Instance = this;
            EventMgr.RegisterEvent(this);
            BlackMask.Modulate = new(BlackMask.Modulate, 0);
            BlackMask.MouseFilter = Control.MouseFilterEnum.Ignore;
            LoadConfig();
            HideRichHint();
            Datas.Boost();
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
            var currMap = root.WorldControl.GetChild(0) as Control;
            if (currMap != null) SaveMapData(currMap);
            if (sceneUrl != currMap.SceneFilePath.SimplifyPath())
            {
                if (save.MapSaveData.ContainsKey(currMap.Name))
                {
                    if (save.MapSaveData[currMap.Name].ContainsKey("enemies_died"))
                    {
                        save.MapSaveData[currMap.Name].Remove("enemies_died");
                    }
                }
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
            HealthLab.Text = newhealth.ToString() + "/" + (player?.status.MaxHealth.ToString() ?? "0");
        }

        public void PlayerMaxHealthChanged(double newhealth)
        {
            healthBar.MaxValue = newhealth;
            HealthLab.Text = (player?.status.Health.ToString() ?? "0") + "/" + (player?.status.MaxHealth.ToString() ?? "0");
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
                    PlayerMaxHealthChanged((double)datas[0]);
                    break;
                case "RootReady":
                    root = (RootScene)datas[0];
                    break;
                case "EnemyDied":
                    RecordEnemyDied(datas[0] as Enemy);
                    break;
                case "SkillUpdate":
                    UpdateSkillSlots();
                    break;
            }
        }

        public void UpdateLeftUpperBox()
        {
            healthBar.MaxValue = player?.status?.MaxHealth ?? 1;
            healthBar.Value = player?.status?.Health ?? 1;
            healthBar.MinValue = 0;
            HealthLab.Text = (player?.status?.Health ?? 1) + "/" + (player?.status.MaxHealth.ToString() ?? "0");
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
            if (save == null || map == null) return;
            save.MapSaveData ??= new();
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
                        enemy.OnDeathFunc();
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
            foreach (Crate crate in GetTree().GetNodesInGroup("crates").Cast<Crate>())
            {
                crate.IsOpen = save.CrateOpened.Contains(crate.Name);
            }
        }

        public void SaveGame()
        {
            save.currMapPath = (root.WorldControl.GetChild(0) as Control).SceneFilePath.GetFile().GetBaseDir();
            SaveMapData(root.WorldControl.GetChild(0) as Control);
            player.status.Health = player.status.MaxHealth;
            save.PlayerData = player.ToDict();
            save.SkillSlot = SkillSlot;
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
                currMapPath = data.GetValueOrDefault("currMapPath", "res://Scenes/ForestMap.tscn").AsString(),
                PlayerData = data.GetValueOrDefault("PlayerData", new Godot.Collections.Dictionary<string, Variant>()).AsGodotDictionary<string, Variant>(),
                MapSaveData = data.GetValueOrDefault("MapSaveData", new Godot.Collections.Dictionary<string, Variant>()).AsGodotDictionary<string, Godot.Collections.Dictionary<string, Variant>>(),
                ActivedSavePoints = data.GetValueOrDefault("ActivedSavePoints", new Godot.Collections.Dictionary<string, Variant>()).AsGodotDictionary<string, string>(),
                GlobalSaveData = data.GetValueOrDefault("GlobalSaveData", new Godot.Collections.Dictionary<string, Variant>()).AsGodotDictionary<string, Variant>(),
                SkillSlot = data.GetValueOrDefault("SkillSlot", new Array<string>()).AsGodotArray<string>(),
            };
            var CrateOpened = data.GetValueOrDefault("CrateOpened", new Array<string>()).AsGodotArray<string>();
            foreach (var item in CrateOpened)
            {
                save.CrateOpened.Add(item);
            }
            SkillSlot = save.SkillSlot;
            UpdateSkillSlots();
            await ChangeSceneAsync(save.currMapPath, true);
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

        public static void SaveConfig()
        {
            ConfigFile config = new();
            config.SetValue("audio", "master", SoundManager.GetVolume((int)SoundManager.AudioBusEnum.Master));
            config.SetValue("audio", "sfx", SoundManager.GetVolume((int)SoundManager.AudioBusEnum.SFX));
            config.SetValue("audio", "bgm", SoundManager.GetVolume((int)SoundManager.AudioBusEnum.BGM));
            config.Save(CONFIG_PATH);
        }

        public static void LoadConfig()
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

        public Array<string> SkillSlot = new();
        public void SetSkill(int skillslot, string skillname)
        {
            if (SkillSlot.Count < skillslot + 1)
            {
                for (int i = SkillSlot.Count; i <= skillslot; i++)
                {
                    SkillSlot.Add(null);
                }
            }
            SkillSlot[skillslot] = skillname;
            //UpdateSkillSlots();
            EventMgr.DispatchEvent("SkillUpdate");
        }
        public void UpdateSkillSlots()
        {
            foreach (SkillBtn skillBtn in SkillSlotContainer.GetChildren().Cast<SkillBtn>())
            {
                var skillname = SkillSlot.Count > skillBtn.GetIndex() ? SkillSlot[skillBtn.GetIndex()] : null;
                var dict = SkillManager.Instance.SkillDict;
                if (skillname != null && dict.ContainsKey(skillname))
                {
                    skillBtn.SetSkill(dict[skillname]);
                }
                else
                {
                    skillBtn.SetSkill(null);
                }
            }
        }

        public override void _Process(double delta)
        {
            if (richHintBg.Visible == true)
            {
                float PositionX = GetViewport().GetMousePosition().X + 5;
                float PositionY = GetViewport().GetMousePosition().Y + 5;
                richHintBg.Position = new Vector2((PositionX + richHintBg.Size.X) > 384 ? PositionX - 20 - richHintBg.Size.X : PositionX, (PositionY + richHintBg.Size.Y) > 216 ? PositionY - richHintBg.Size.Y : PositionY);
                richHintBg.Size = richHint.Size;
            }
        }

        private Action richHintAction;
        public void ShowRichHint(string hint)
        {
            richHintAction?.Invoke();
            richHintAction = new Action(() =>
            {
                if (hint.Length > 50)
                {
                    richHint.Size = new(200, 0);
                }
                else
                {
                    richHint.Size = new(80, 0);
                }
                richHint.Text = hint;
                richHintBg.Visible = true;
            });
            richHintTimer.Timeout += richHintAction;
            richHintTimer.WaitTime = 0.1f;
            richHintTimer.OneShot = true;
            richHintTimer.Start();
        }
        public void HideRichHint()
        {
            richHintAction?.Invoke();
            richHintTimer.Stop();
            richHint.Text = "";
            richHintBg.Visible = false;
        }
    }
}
