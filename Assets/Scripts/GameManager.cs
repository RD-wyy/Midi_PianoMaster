using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI_1: ���ý��� ���ȷ����ť ����InitMidi()

//UI_2: �ȴ���ʼ���� �����ʼ��ť ����StartGame()

//UI_3: ��Ϸ���н��� *����������ͣ����������������*

//UI_4: ��Ϸ�������� ���������ť �ص����ý���

namespace midi_piano
{
    public class GameManager : MonoBehaviour
    {
        public GameObject keysGenerator;
        public GameObject midiReader;
        public GameObject targetsGenerator;
        
        public GameObject canvas_Setting;
        public GameObject canvas_Readying;

        private string[] paths = {"./Assets/Midis/midi_1.mid", 
                                "./Assets/Midis/midi_2.mid"
        };
        private int pathID;

        public AudioSource audioSource;
        public List<AudioClip> audioClips;

        private bool isPlaying;
        private float timeSincePlay;


        void Start()
        {
            canvas_Readying.SetActive(false);
            canvas_Setting.SetActive(true);
            canvas_Setting.GetComponentInChildren<Dropdown>().onValueChanged.AddListener(ChangePath);

            //isPlaying = false;
            //timeSincePlay = 0;
        }

        void Update()
        {
                
        }

        //ѡ��dropdown����
        public void ChangePath(int value)
        {
            pathID = value;
            //Debug.Log(pathID);
        }

        //��Ŀѡ�����ʼ��
        public void InitMidi()
        {
            canvas_Setting.SetActive(false);
            canvas_Readying.SetActive(true);

            keysGenerator.GetComponent<KeysGenerator>().InitKeys();
            midiReader.GetComponent<MidiReader>().Read(paths[pathID]);
            targetsGenerator.GetComponent<TargetsGenerator>().InitTargets(midiReader.GetComponent<MidiReader>().midiEvents);
            
            audioSource.clip = audioClips[pathID];
        }

        //��ʼ��Ϸ
        public void StartGame()
        {
            canvas_Readying.SetActive(false);

            //isPlaying = true;
            audioSource.Play();
            targetsGenerator.GetComponent<TargetsGenerator>().StartTargets();
        }
    }
}
