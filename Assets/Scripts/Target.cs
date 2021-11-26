using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float time_start;
    public float time_end;
    public float time;
    public int noteId;

    public Target(int id, float start)
    {
        time_start = start;
        noteId = id;
    }

    public void SetEndTime(float end)
    {
        time_end = end;
        time = time_end - time_start;
    }

    void Start()
    {
        
    }

    void Update()
    {
    
    }

}
