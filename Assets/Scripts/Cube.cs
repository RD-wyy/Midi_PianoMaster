using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float time_start;
    public float time_end;
    public float time;
    public int noteId;
    public bool isMidiIn;

    private float speed;

    void Start()
    {
        speed = 5.0f;
        isMidiIn = false;
        //gameObject.AddComponent<Collider>();
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
    }

    public void SetInfo(float t_s, float t_e, int n)
    {
        time_start = t_s;
        time_end = t_e;
        noteId = n;
        time = time_end - time_start;
    }
}
