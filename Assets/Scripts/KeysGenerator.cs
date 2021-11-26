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

        //C1 - B5: ��35���׼� 25���ڼ�
        public void InitKeys()
        {
            Vector3 position = this.transform.position;
            bool keyTypePre = false;

            for (int i = 36; i < 36 + keysCount; i++)
            {
                //�жϺڰ׼�
                bool keyType = isBlackKey[(i - 36) % 12];
                GameObject prefab = keyType ? blackKey : whiteKey;
                //ȷ��λ��
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
                //����ʵ��
                GameObject keyTmp = GameObject.Instantiate<GameObject>(prefab,
                    transform.position + position, Quaternion.identity, transform);
                //д����Ϣ
                keyTmp.GetComponent<Key>().id = i;
                keyTmp.GetComponent<Key>().type = keyType;
                keyTmp.name = "NO." + i;
                keysList.Add(keyTmp);
                keyTypePre = keyType;
            }

        }
    }
}
