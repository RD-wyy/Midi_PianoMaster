using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class Key : MonoBehaviour
{
    public int id;
    public bool type; //0:°×¼ü; 1:ºÚ¼ü
    private enum statusList { spare, press, release, pressed, hold };
    public int status;

    private BoxCollider collider;
    private bool isCubeEnter;
    private Cube cube;


    void Start()
    {
        status = (int)statusList.spare;
        collider = transform.GetComponent<BoxCollider>();
        isCubeEnter = false;
    }

    void Update()
    {
        if (MidiMaster.GetKeyDown(id))
            KeyDown();
        if (MidiMaster.GetKeyUp(id))
            KeyUp();
        switch (status)
        {
            case (int)statusList.press:
                transform.Rotate(-0.3f, 0, 0);
                if (transform.rotation.x < -0.02f)
                    status = (int)statusList.pressed;
                break;
            case (int)statusList.release:
                transform.Rotate(0.2f, 0, 0);
                if (transform.rotation.x >= 0)
                    status = (int)statusList.spare;
                break;
        }

    }

    public void KeyDown()
    {
        Debug.Log("key down: " + id);
        status = (int)statusList.press;
        //
        if (isCubeEnter && !cube.isMidiIn)
        {
            cube.isMidiIn = true;
        }
    }

    public void KeyUp()
    {
        Debug.Log("key up: " + id);
        status = (int)statusList.release;
    }

    private void OnTriggerEnter(Collider other)
    {
        isCubeEnter = true;
        cube = other.transform.parent.GetComponent<Cube>();
    }

    private void OnTriggerExit(Collider other)
    {
        isCubeEnter = false;
        
        if (!cube.isMidiIn)
            Debug.Log("miss " + cube.noteId);
    }
}
