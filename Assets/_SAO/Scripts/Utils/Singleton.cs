using UnityEngine;

public class Singleton<t> : MonoBehaviour where t : Component
{
    public static t Instance;

    internal virtual void Awake()
    {
        if (Instance == null)
            Instance = this as t;
        else
            Destroy(this.gameObject);
    }
}