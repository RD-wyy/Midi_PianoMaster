using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI_1: 设置界面 点击确定按钮 调用InitMidi()

//UI_2: 等待开始界面 点击开始按钮 调用StartGame()

//UI_3: 游戏进行界面 *后续增加暂停、继续、结束功能*

//UI_4: 游戏结束界面 点击结束按钮 回到设置界面

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

        //选曲dropdown监听
        public void ChangePath(int value)
        {
            pathID = value;
            //Debug.Log(pathID);
        }

        //曲目选择与初始化
        public void InitMidi()
        {
            canvas_Setting.SetActive(false);
            canvas_Readying.SetActive(true);

            keysGenerator.GetComponent<KeysGenerator>().InitKeys();
            midiReader.GetComponent<MidiReader>().Read(paths[pathID]);
            targetsGenerator.GetComponent<TargetsGenerator>().InitTargets(midiReader.GetComponent<MidiReader>().midiEvents);
            
            audioSource.clip = audioClips[pathID];
        }

        //开始游戏
        public void StartGame()
        {
            canvas_Readying.SetActive(false);

            //isPlaying = true;
            audioSource.Play();
            targetsGenerator.GetComponent<TargetsGenerator>().StartTargets();
        }
    }
}
