using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCircleManager : MonoBehaviour
{
    [SerializeField] private StatusCircle[] circles;

    public void SetStatusComplete(int circleIndex) { circles[circleIndex].SetStatusComplete(); }

    public void SetStatusFinished() 
    {
        for (int i = 0; i < circles.Length; i++)
        {
            circles[i].SetStatusFinished();
        }
    }
}
