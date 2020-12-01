using System;
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

        public byte Status => (byte)(0xb0 | Channel);

        public byte controlChangeNumber;
        public byte data;
        public ControlChangeEvent(){}
        public ControlChangeEvent(uint ticks, byte channel, byte controlChangeNumber, byte data):base(ticks)
        {
            Channel = channel;
            this.controlChangeNumber = controlChangeNumber;
            this.data = data;
        }
    }
}

