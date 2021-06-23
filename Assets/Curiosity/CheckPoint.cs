using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public RaycastAgent agent;
    public int checkPointId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            this.agent.EnterCheckPoint(this.checkPointId);
        }
    }
}
