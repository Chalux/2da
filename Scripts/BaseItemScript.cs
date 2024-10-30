using Godot;

namespace da.Scripts
{
    public partial class BaseItemScript : RefCounted
    {
        public virtual void Use()
        {

        }
        public virtual bool CheckCanUse()
        {
            return true;
        }
    }
}
