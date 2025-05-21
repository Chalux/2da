namespace da.Scripts.Interfaces
{
    internal interface IOpenable
    {
        /// <summary>
        /// 用于读档的时候，将状态设置为打开，不需要做过渡操作，直接设置即可
        /// </summary>
        public abstract void SetToOpenedState();

        /// <summary>
        /// 用于读档的时候，将状态设置为关闭，不需要做过渡操作，直接设置即可
        /// </summary>
        public abstract void SetToClosedState();

        /// <summary>
        /// 用于打开的时候，做过渡操作，比如播放动画等
        /// </summary>
        public abstract void OnOpen();

        /// <summary>
        /// 用于关闭的时候，做过渡操作，比如播放动画等
        /// </summary>
        public abstract void OnClose();
    }
}
