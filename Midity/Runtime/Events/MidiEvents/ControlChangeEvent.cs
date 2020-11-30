using UnityEngine;

namespace Midity
{
    [System.Serializable]
    public class ControlChangeEvent : MTrkEvent
    {
        [SerializeField]
        private byte channel;
        public byte Channel
        {
            get => channel;
            set => channel = (byte)Mathf.Clamp(value, 0, 16);
        }

        public byte controlChangeNumber;
        public byte data;
        public ControlChangeEvent(){}
        public ControlChangeEvent(uint ticks, byte status, byte controlChangeNumber, byte data)
        {
            this.ticks = ticks;
            Channel = (byte)(status & 0x0f);
            this.controlChangeNumber = controlChangeNumber;
            this.data = data;
        }
    }
}

