using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
}

public enum SpawnPosition 
{
    LeftHand,
    RightHand,
    Head,
    Body,
    Legs,
    Feet,
    InFront,
    Position
}


public class ItemSpawner : Singleton<ItemSpawner>
{
    private Vector3 spawnPos, spawnRot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    internal void SetSpawnPosition(Vector3 pos, Vector3 rot)
    {
        spawnPos = pos;
        spawnRot = rot;
    }

    public GameObject SpawnItem(SpawnPosition position, GameObject obj, Transform parent = null)
    {
        GameObject instance = Instantiate(obj, parent);
        switch (position)
        {
            case SpawnPosition.LeftHand:
                break;
            case SpawnPosition.RightHand:
                break;
            case SpawnPosition.Head:
                break;
            case SpawnPosition.Body:
                break;
            case SpawnPosition.Legs:
                break;
            case SpawnPosition.Feet:
                break;
            case SpawnPosition.InFront:
                break;
            case SpawnPosition.Position:
                instance.transform.position = spawnPos;
                instance.transform.Rotate(spawnRot);
                break;
            default:
                break;
        }

        return instance;
    }

    public void DespawnItem(GameObject obj)
    {
        Destroy(obj);
    }
}
