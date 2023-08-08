using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageHandler : MonoBehaviour
{
    public void OnMessageReceived(string message)
    {
        Debug.Log("Message Received: " + message);
    }

    public void SendMessage()
    {
        Debug.Log("Message Sent");
    }

    public void SendResponse()
    {
        Debug.Log("Response Sent");
    }
    
    internal void CreateMessage()
    {
        
    }
}
