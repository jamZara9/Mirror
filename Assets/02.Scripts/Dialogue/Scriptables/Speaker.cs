using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Story/New Speaker")]
[System.Serializable]
public class Speaker : ScriptableObject
{
    public string speakerName; // 화면에 표시될 이름.
    public Color nameColor; // 이름 텍스트 색상.
    public Sprite[] sprites; // 캐릭터 이미지 저장 배열.
    public SpriteController prefab; // 해당 캐릭터의 스크립트가 저장된 프리펩.
}
