using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace midi_piano
{
    public class KeysGenerator : MonoBehaviour
    {
        //prefab
        public GameObject whiteKey;
        public GameObject blackKey;
        public Vector3[] positions = new Vector3[100];

        private Key key;

        private int keysCount;
        private List<GameObject> keysList;
        private bool[] isBlackKey = {false, true, false, true, false, false, true,
        false, true, false, true, false};

        void Start()
        {
            keysCount = 60;
            keysList = new List<GameObject>();
        }

        void Update()
        {

        }

        //C1 - B5: 共35个白键 25个黑键
        public void InitKeys()
        {
            Vector3 position = this.transform.position;
            bool keyTypePre = false;

            for (int i = 36; i < 36 + keysCount; i++)
            {
                //判断黑白键
                bool keyType = isBlackKey[(i - 36) % 12];
                GameObject prefab = keyType ? blackKey : whiteKey;
                //确定位置
                if (keyType == keyTypePre)
                {
                    position.x += 1;
                }
                else if (keyType)
                {
                    position.x += 0.5f;
                    position.y += 0.3f;
                }
                else
                {
                    position.x += 0.5f;
                    position.y -= 0.3f;
                }
                positions[i] = position;
                //创建实例
                GameObject keyTmp = GameObject.Instantiate<GameObject>(prefab,
                    transform.position + position, Quaternion.identity, transform);
                //写入信息
                keyTmp.GetComponent<Key>().id = i;
                keyTmp.GetComponent<Key>().type = keyType;
                keyTmp.name = "NO." + i;
                keysList.Add(keyTmp);
                keyTypePre = keyType;
            }

        }
    }
}
