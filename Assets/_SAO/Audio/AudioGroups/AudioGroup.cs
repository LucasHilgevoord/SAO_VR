using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioGroup", menuName = "DataObjects/AudioGroup", order = 2)]
public class AudioGroup : ScriptableObject
{
    public string id;
    [TextArea(5, 20)]
    public string description;
    public List<AudioClip> audioClips;
}
