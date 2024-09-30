using Godot;
using Godot.Collections;

namespace da.Scripts
{
    public partial class SaveData : RefCounted
    {
        /// <summary>
        /// 外部key值对应地图的Name，内部键值自己定义意义
        /// </summary>
        public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> MapSaveData = new();

        public string currMapPath = "res://Scenes/ForestMap.tscn";

        public Godot.Collections.Dictionary<string, Variant> PlayerData = new();

        public Dictionary<string, Variant> ToList()
        {
            return new Dictionary<string, Variant>()
            {
                { "MapSaveData", MapSaveData },
                { "currMapPath", currMapPath },
                { "PlayerData", PlayerData }
            };
        }
    }
}
