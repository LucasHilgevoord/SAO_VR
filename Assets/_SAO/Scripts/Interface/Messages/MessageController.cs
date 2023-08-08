using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageController : MonoBehaviour
{
    public void OnMessageReceived(int userID, string message)
    {
        Debug.Log("Message Received: " + message);
    }
}
