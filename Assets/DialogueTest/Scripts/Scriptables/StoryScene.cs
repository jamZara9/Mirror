using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryScene", menuName = "Story/New Story Scene")]
[System.Serializable]
public class StoryScene : ScriptableObject
{
    public bool isMainStory = true; // 필수 스토리인가?
    public bool isMovableScene; // 움직일 수 있는 스토리 진행 방식인가?
    
    public List<Sentence> sentences; // 문장 리스트.
    public Sprite background; // 기본 배경 이미지.
    public AudioClip backgroundMusic; // BGM Clip.
    public StoryScene nextScene; // 바로 이어져 실행할 다음 스토리.
    
    public TextAsset csvFile; // 해당 스토리의 CSV 파일.
    
    public string summaryText; // 요약글.

    // 문장
    [System.Serializable]
    public struct Sentence
    {
        public AudioClip audioClip; // 사운드 이펙트.
        
        public string text; // 대사.
        public Speaker speaker; // 화자.

        public Sprite background; // 바뀔 배경 이미지.
        
        public List<Action> actions; // 스프라이트 액션 리스트.

        public float nextSentenceDelay; // 다음 문장 딜레이.
        
        // 스프라이트 액션.
        [System.Serializable]
        public struct Action
        {
            public Speaker speaker; // 액션을 실행할 스프라이트.
            public Type actionType; // 액션 타입.
            
            public int spriteIndex; // 바뀔 때의 스프라이트 이미지 인덱스.
            
            public Vector2 coords; // 움직일 때의 목적지.
            public float moveSpeed; // 움직일 때의 소요 시간.

            public int width; // 크기 변환 시 너비.
            public int height; // 크기 변환 시 높이.
            public float scaleSpeed; // 크기 변환 시 소요 시간.

            // 액션 타입.
            [System.Serializable]
            public enum Type
            {
                None, Appear, Move, DisAppear, Change, Scale, First, Bright, AllDark, AllBright
            }
        }
    }

}
