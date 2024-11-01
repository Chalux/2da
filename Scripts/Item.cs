using da.Scripts.Objects;
using Godot;

namespace da.Scripts
{
    public partial class Item : RefCounted
    {
        public int id = 0;
        public string name = "未命名";
        public string description = "无描述";
        public string IconPath = "res://Resources/Assets/skillicons/potion-ball.png";
        public int StackCount = 0;
        public int type = 1;
        public BaseItemScript script;
        public Item(int id)
        {
            this.id = id;
            if (Datas.Items.ContainsKey(id))
            {
                var item = Datas.Items[id];
                name = item.name;
                description = item.description;
                IconPath = item.IconPath;
                if (item.ScriptPath != null && item.ScriptPath != "")
                {
                    var scriptres = ResourceLoader.Load<CSharpScript>(item.ScriptPath);
                    if (scriptres != null)
                    {
                        script = (BaseItemScript)scriptres.New();
                    }
                }
                type = item.type;
            }
        }

        public void Use()
        {
            switch (type)
            {
                default:
                case 1:
                    if (StackCount <= 0)
                    {
                        GameGlobal.Instance.player?.Bag.Remove(this);
                        return;
                    }
                    if (script != null)
                    {
                        script.Use();
                        StackCount--;
                        if (StackCount <= 0)
                        {
                            GameGlobal.Instance.player?.Bag.Remove(this);
                        }
                        EventMgr.DispatchEvent("BagUpdate");
                    }
                    break;
            }
        }
    }
}
