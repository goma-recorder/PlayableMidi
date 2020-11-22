namespace Midity.Playable
{
    [System.Serializable]
    public class MTrkEventHolder<T> where T : MTrkEvent
    {
        public int index;
        public T Event;
        public MTrkEventHolder() { }
        public MTrkEventHolder(int index, T mtrkEvent)
        {
            this.index = index;
            this.Event = mtrkEvent;
        }
    }
}
