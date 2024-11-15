using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts
{
    public partial class Role : CharacterBody2D, IEvent
    {
        public List<Buff> buffs = new();
        public void AddBuff(Buff buff)
        {
            if (!buffs.Contains(buff))
            {
                buffs.Add(buff);
            }
        }

        public virtual void ReceiveEvent(string eventName, params object[] datas)
        {
            foreach (Buff buff in buffs)
            {
                buff.ReceiveEvent(eventName, datas);
            }
        }

        public void RemoveBuff(Buff buff)
        {
            if (buffs.Contains(buff))
            {
                buffs.Remove(buff);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            for (int i = 0; i < buffs.Count; i++)
            {
                Buff buff = buffs[i];
                buff.PhysicsProcess(delta, this);
            }
        }

        public override void _Ready()
        {
            base._Ready();
            if (this is IEvent)
            {
                EventMgr.RegisterEvent(this);
            }
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            if (this is IEvent)
            {
                EventMgr.UnRegisterEvent(this);
            }
        }
    }
}
