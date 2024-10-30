using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace da.Scripts
{
    internal class Datas
    {
        //public static Datas Ins { get; private set; }
        public static Godot.Collections.Dictionary<int, ItemStruct> Items = new();
        public static void Boost()
        {
            string path = $"res://datas/item_info.json";
            if (FileAccess.FileExists(path))
            {
                using var ItemData = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                var json = Json.ParseString(ItemData.GetAsText());
                if ((json as object) != null)
                {
                    foreach (var item in json.AsGodotArray<Variant>())
                    {
                        var dict = item.AsGodotDictionary();
                        var tis = new ItemStruct()
                        {
                            id = dict["id"].AsInt32(),
                            name = dict["name"].AsString(),
                            description = dict["description"].AsString(),
                            IconPath = dict["IconPath"].AsString(),
                            ScriptPath = dict["ScriptPath"].AsString(),
                        };
                        Items.TryAdd(dict["id"].AsInt32(), tis);
                    }
                }
            }
        }
    }

    public partial class ItemStruct : RefCounted
    {
        public int id = 0;
        public string name = "";
        public string description = "";
        public string IconPath = "";
        public string ScriptPath = "";
        public int type = 1;
    }
}
