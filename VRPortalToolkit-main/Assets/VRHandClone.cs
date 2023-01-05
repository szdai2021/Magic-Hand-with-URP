using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandClone : MonoBehaviour
{
    public GameObject parent;

    public GameObject VRRoom;
    public GameObject VRRoomClone;

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = parent.transform.rotation;
        this.transform.position = parent.transform.position - (VRRoom.transform.position - VRRoomClone.transform.position);
    }
}

