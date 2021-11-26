using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace midi_piano
{
    public class TargetsGenerator : MonoBehaviour
    {
        public GameObject prefabCube;
        public List<Target> targets = new List<Target>();
        public GameObject keysGenerator;
        public float trackLength;

        private int index;
        private int tempo;
        private int signature;
        private float tickPerSecond;
        private float timeSincePlay;
        private bool isPlaying;
        private Vector3[] positions;
        private Target target;
        private int targetNo;
        private int tick;

        void Start()
        {
            isPlaying = false;
            timeSincePlay = 0;
            index = 0;
            trackLength = 11;
        }

        void Update()
        {
            if (isPlaying)
            {
                timeSincePlay += Time.deltaTime;
                if (target.time_start / tickPerSecond < timeSincePlay + 2)
                {
                    Vector3 position = new Vector3(positions[target.noteId].x, 0, trackLength);
                    //Debug.Log("create " + target.noteId + " time:" + timeSincePlay + " tick:" + target.time_start);
                    GameObject cube = GameObject.Instantiate<GameObject>(prefabCube,
                        position, Quaternion.identity, transform);
                    cube.GetComponent<Cube>().SetInfo(target.time_start, target.time_end, target.noteId);
                    cube.transform.localScale = new Vector3(0.6f, 1, target.time / tickPerSecond);
                    targetNo++;
                    if (targetNo < targets.Count)
                        target = targets[targetNo];
                    else
                        isPlaying = false;
                }
            }
        }

        public void InitTargets(List<MidiEvent> midiEvents)
        {
            tempo = GameObject.Find("Midi Reader").GetComponent<MidiReader>().Tempo;
            signature = GameObject.Find("Midi Reader").GetComponent<MidiReader>().signature[1];
            positions = keysGenerator.GetComponent<KeysGenerator>().positions;
            tick = GameObject.Find("Midi Reader").GetComponent<MidiReader>().tick;
            tickPerSecond = tempo * tick * 4 / signature / 60;
            Debug.Log("tickPerSecond: " + tickPerSecond);

            foreach (MidiEvent tmpEvent in midiEvents)
            {
                if (tmpEvent.status_channal == (byte)0x90 && tmpEvent.velocity > 0)
                //if (tmpEvent.status_channal == (byte)0x90)
                    {
                    Target tmpTarget = new Target(tmpEvent.note, tmpEvent.time);
                    targets.Add(tmpTarget);
                    index++;
                }
                else if (tmpEvent.status_channal == (byte)0x80 || tmpEvent.status_channal == (byte)0x90)
                //else if (tmpEvent.status_channal == (byte)0x80)
                        {
                    for (int i = index - 1; i >= 0; i--)
                    {
                        if (targets[i].noteId == tmpEvent.note)
                        {
                            targets[i].SetEndTime(tmpEvent.time);
                            //Debug.Log(targets[i].noteId + ": " + targets[i].time);
                            break;
                        }
                    }
                }
            }
        }

        public void StartTargets()
        {
            isPlaying = true;
            targetNo = 0;
            target = targets[targetNo];
        }

        //private float Tick2Second()
        //{
        //    //1个四分音符等于120tick
        //    //60秒 tempo拍  1拍为signature
        //    //求每tick多少秒
        //    return tempo * 120 * 4 / signature / 60;
        //}
    }
}
