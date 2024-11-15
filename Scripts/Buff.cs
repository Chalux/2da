using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts
{
    public partial class Buff : RefCounted
    {
        private float _timeout = -1;
        private int _count = -1;
        protected Role _owner;
        private bool isTimeInfinite = false;
        private bool isCountInfinite = false;
        public float Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                if (!isTimeInfinite && _timeout <= 0)
                {
                    RemoveSelf();
                }
            }
        }
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                if (_count <= 0 && !isCountInfinite)
                {
                    RemoveSelf();
                }
            }
        }
        public Buff(Role owner, float timeout, int count)
        {
            _owner = owner;
            Timeout = timeout;
            Count = count;
            isTimeInfinite = timeout == -1;
            isCountInfinite = count == -1;
        }
        public virtual void PhysicsProcess(double delta, Role owner)
        {

        }
        public virtual void ReceiveEvent(string eventName, params object[] datas)
        {

        }
        public void RemoveSelf()
        {
            BeforeRemoved();
            _owner.RemoveBuff(this);
        }
        protected virtual void BeforeRemoved()
        {

        }
    }
}
