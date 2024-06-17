using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryScene", menuName = "Story/New Story Scene")]
[System.Serializable]
public class StoryScene : ScriptableObject
{
    public List<Sentence> sentences;
    public Sprite background;
    public AudioClip backgroundMusic;
    public StoryScene nextScene;
    public TextAsset csvFile;

    public string summaryText;
    
    public bool isMovableScene;

    [System.Serializable]
    public struct Sentence
    {
        public AudioClip audioClip;
        
        public string text;
        public Speaker speaker;

        public Sprite background;
        
        public List<Action> actions;

        public float nextSentenceDelay;
        
        [System.Serializable]
        public struct Action
        {
            public Speaker speaker;
            public Type actionType;
            
            public int spriteIndex;
            
            public Vector2 coords;
            public float moveSpeed;

            public int width;
            public int height;
            public float scaleSpeed;

            [System.Serializable]
            public enum Type
            {
                None, Appear, Move, DisAppear, Change, Scale, First, Bright
            }
        }
    }

}
