using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace midi_piano
{
    public class MidiReader : MonoBehaviour
    {
        FileStream midiFile;
        public bool flag = true; //标记midi文件是否为标准格式
        float timeNow = 0;

        //standard byte
        byte[] MThd = new byte[] { 0x4d, 0x54, 0x68, 0x64 };
        byte[] MTrk = new byte[] { 0x4d, 0x54, 0x72, 0x6b };
        byte[] HeadChunk_Length = new byte[] { 0x00, 0x00, 0x00, 0x06 };
        byte[] event_EndTrack = new byte[] { 0xff, 0x2f, 0x00 };

        //Info
        byte[] format = new byte[2];
        byte[] ntrks = new byte[2];
        byte[] division = new byte[2];
        int trackChunk_Count = 0; //Track Chunk数量
        public int[] signature = new int[2]; //拍号
        public int Tempo = 0; //速度
        public int tick = 0;
        public List<MidiEvent> midiEvents = new List<MidiEvent>();

        void Start()
        {
            
        }

        public void Read(string path)
        {
            midiFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            //读取信息
            ReadHeaderChunk();
            for (int i = 0; i < trackChunk_Count; i++)
            {
                ReadTrackChunk(i);
            }
            //检验
            if (flag == true)
            {
                ShowInfo();
            }
            else
            {
                Debug.Log("false");
            }

            midiFile.Close();
        }

        //读取Head Chunk
        void ReadHeaderChunk()
        {
            byte[] _chunkType = new byte[4];
            midiFile.Read(_chunkType, 0, 4);
            if (!BytesCompare_Step(_chunkType, MThd))
                flag = false;

            byte[] _headChunkLength = new byte[4];
            midiFile.Read(_headChunkLength, 0, 4);
            if (!BytesCompare_Step(_headChunkLength, HeadChunk_Length))
                flag = false;

            midiFile.Read(format, 0, 2);
            midiFile.Read(ntrks, 0, 2);
            midiFile.Read(division, 0, 2);
            BitArray divisionArray = new BitArray(division);
            tick = BitArrayToInt(BitArrayReverse(divisionArray));

            trackChunk_Count = (int)(ntrks[1] | ntrks[0] << 8);
        }

        //读取Track Chunk
        void ReadTrackChunk(int trackChunkId)
        {
            byte[] _chunkType = new byte[4];
            midiFile.Read(_chunkType, 0, 4);
            if (!BytesCompare_Step(_chunkType, MTrk))
                flag = false;

            byte[] _trackChunkLength = new byte[4];
            midiFile.Read(_trackChunkLength, 0, 4);
            int bytes_Count = (int)(_trackChunkLength[3] | _trackChunkLength[2] << 8 |
                _trackChunkLength[1] << 16 | _trackChunkLength[0] << 24);

            int byteNow = 0;
            byte[] buffer_status_pre = new byte[1];

            while (byteNow < bytes_Count)
            {
                //delta-time
                byteNow += ReadTime();

                //event
                byte[] buffer_status = new byte[1];
                byte[] buffer = new byte[2];
                //midiFile.Read(buffer, 0, 3);
                midiFile.Read(buffer_status, 0, 1);
                byteNow += 1;
                int lenToRead = 0;
                if (buffer_status[0] == (byte)0xff)
                {
                    //meta-event
                    midiFile.Read(buffer, 0, 2);
                    buffer_status_pre[0] = buffer_status[0];
                    byteNow += 2;
                    if (buffer[0] == (byte)0x2f && buffer[1] == (byte)0x00)
                        continue;
                    switch (buffer[0])
                    {
                        case (byte)0x03: //轨道名称
                            lenToRead = (int)buffer[1];
                            byte[] _trackName = new byte[lenToRead];
                            midiFile.Read(_trackName, 0, lenToRead);
                            byteNow += lenToRead;
                            break;
                        case (byte)0x51: //读取速度
                            byte[] _tempo = new byte[3];
                            midiFile.Read(_tempo, 0, 3);
                            Tempo = (int)(_tempo[2] | _tempo[1] << 8 | _tempo[0] << 16);
                            Tempo = 1000000 * 60 / Tempo;
                            byteNow += 3;
                            break;
                        case (byte)0x58: //读取拍号
                            byte[] _signature = new byte[4];
                            midiFile.Read(_signature, 0, 4);
                            signature[0] = (int)_signature[0];
                            signature[1] = (int)Math.Pow(2,(double)_signature[1]);
                            byteNow += 4;
                            break;
                        default:
                            flag = false;
                            break;
                    }
                }
                else if (buffer_status[0] == (byte)0xf0)
                {
                    //sysex event
                    midiFile.Read(buffer, 0, 2);
                    buffer_status_pre[0] = buffer_status[0];
                    byteNow += 2;
                }
                else if (buffer_status[0] == (byte)0x90 || buffer_status[0] == (byte)0x80)
                {
                    //midi event
                    midiFile.Read(buffer, 0, 2);
                    buffer_status_pre[0] = buffer_status[0];
                    byteNow += 2;
                    MidiEvent midiEvent = new MidiEvent(buffer_status[0], buffer[0], buffer[1], timeNow);
                    midiEvents.Add(midiEvent);
                    //if (buffer_status[0] == (byte)0x90)
                    //    Target target = new Target(midiEvent.note, midiEvent.time);
                    //else

                    continue;
                }
                else
                {
                    //midi event; status same
                    midiFile.Read(buffer, 0, 1);
                    byteNow += 1;
                    MidiEvent midiEvent = new MidiEvent(buffer_status_pre[0], buffer_status[0], buffer[0], timeNow);
                    midiEvents.Add(midiEvent);
                    continue;
                }
            }

        }

        int ReadTime()
        {
            float time = 0;
            byte[] bytes = new byte[4];
            byte[] buffer = new byte[1];
            midiFile.Read(buffer, 0, 1);
            int byteLen;
            for (byteLen = 1; byteLen <= 4; byteLen++)
            {
                if (buffer[0] < (byte)0x80)
                {
                    bytes[byteLen - 1] = buffer[0];
                    break;
                }
                else
                {
                    bytes[byteLen - 1] = buffer[0];
                    midiFile.Read(buffer, 0, 1);
                }
            }
            
            BitArray array = new BitArray(bytes);
            BitArray array7 = Array8ToArray7(array, byteLen);
            time = BitArrayToInt(array7);
            timeNow += time;
            return byteLen;
        }

        void ShowInfo()
        {
            Debug.Log("Track Chunk 数量: " + trackChunk_Count);
            Debug.Log("拍号: " + signature[0] + "/" + signature[1]);
            Debug.Log("速度: " + Tempo);
            Debug.Log("tick: " + tick);
            //foreach (MidiEvent midiEvent in midiEvents)
            //{
            //    midiEvent.ShowInfo();
            //}
        }

        private bool BytesCompare_Step(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; ++i)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        private int BitArrayToInt(BitArray array)
        {
            int ret = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                    ret += (int)Math.Pow(2, array.Length - i - 1);
            }
            return ret;
        }

        private void ShowBitArray(BitArray array)
        {
            string ret = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == true)
                    ret += "1";
                else
                    ret += "0";
            }
            Debug.Log(ret);
        }

        private BitArray Array8ToArray7(BitArray array, int byteLen)
        {
            BitArray ret = new BitArray(byteLen * 7);
            int num;
            for (num = 0; num < byteLen; num++)
            {
                for (int i = 7 * num, j = 8 * num + 6; i <= 7 * num + 6; i++, j--)
                {
                    ret[i] = array[j];
                }
            }
            return ret;
        }

        private BitArray BitArrayReverse(BitArray array)
        {
            BitArray ret = new BitArray(16);
            for (int i = 0; i < 8; i++)
                ret[i] = array[8 - i - 1];
            for (int i = 0; i < 8; i++)
                ret[i + 8] = array[16 - i - 1];
            return ret;
        }

    }

}
