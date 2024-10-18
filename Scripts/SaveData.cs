using Godot;
using Godot.Collections;

namespace da.Scripts
{
    public partial class SaveData : GodotObject
    {
        /// <summary>
        /// 外部key值对应地图的Name，内部键值自己定义意义
        /// </summary>
        public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> MapSaveData = new();

        public string currMapPath = "res://Scenes/ForestMap.tscn";

        public Godot.Collections.Dictionary<string, Variant> PlayerData = new();

        public Dictionary<string, Variant> GlobalSaveData = new();

        /// <summary>
        /// key值为存档点的Name值，value为地图的路径，用于跨地图传送
        /// </summary>
        public Dictionary<string, string> ActivedSavePoints = new();

        public Dictionary<string, Variant> ToList()
        {
            return new Dictionary<string, Variant>()
            {
                { "MapSaveData", MapSaveData },
                { "currMapPath", currMapPath },
                { "PlayerData", PlayerData },
                { "ActivedSavePoints", ActivedSavePoints },
                { "GlobalSaveData", GlobalSaveData }
            };
        }
    }
}
