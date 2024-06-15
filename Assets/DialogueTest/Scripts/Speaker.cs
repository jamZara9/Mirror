using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Story/New Speaker")]
[System.Serializable]
public class Speaker : ScriptableObject
{
    public string speakerName;
    public Color nameColor;
    public Sprite[] sprites;
    public SpriteController prefab;
}
