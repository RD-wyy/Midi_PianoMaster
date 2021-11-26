using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace midi_piano
{
    public class MidiEvent : MonoBehaviour
    {
        public byte status_channal;
        byte data_1;
        byte data_2;

        //enum status { };
        public int channal;
        public int note;
        public int velocity;
        public float time;

        public MidiEvent(byte _status_channal, byte _data_1, byte _data_2, float _time)
        {
            status_channal = _status_channal;
            data_1 = _data_1;
            data_2 = _data_2;
            time = _time;
            note = _data_1;
            velocity = (int)data_2;
        }

        ~MidiEvent(){ }

        public void ShowInfo()
        {
            Debug.Log(time + ": " + status_channal.ToString("X2") + " " + 
                data_1.ToString("X2") + " " + data_2.ToString("X2"));
        }
    }
}

