using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image background;
    
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;

    private int _sentenceIndex = -1;
    public StoryScene currentScene;

    [SerializeField] private BackGroundController backGroundController;

    public float typeDelay = 0.05f;

    private Dictionary<Speaker, SpriteController> _sprites;
    public GameObject spritesPrefab;

    private AudioSource _audioSource;

    private bool _isDelayFinish = true;

    private Speaker _lastSpeaker = null;
    private Sprite _lastBackground = null;
    
    private const int _CSV_SPEAKER_INDEX = 0;
    private const int _CSV_TEXT_INDEX = 1;
    private const int _CSV_BACKGROUND_INDEX = 2;
    private const int _CSV_AUDIOCLIP_INDEX = 3;
    private const int _CSV_NEXTDELAY_INDEX = 4; 

    private const int _CSV_ACTION_INDEX = 5;
    private const int _CSV_ACTION_TYPE_INDEX = 6;
    private const int _CSV_ACTION_SPRITEINDEX_INDEX = 7;
    private const int _CSV_ACTION_COORDS_X_INDEX = 8;
    private const int _CSV_ACTION_COORDS_Y_INDEX = 9;
    private const int _CSV_ACTION_MOVESPEED_INDEX = 10;

    private enum State
    {
        Playing, Completed
    }
    private State _state = State.Completed;

    void Awake()
    {
        _sprites = new Dictionary<Speaker, SpriteController>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayScene()
    {
        _sentenceIndex = -1;
        
        _audioSource.clip = currentScene.backgroundMusic;
        _audioSource.Play();
        
        PlayNextSentence();
    }

    public void ParseCSVFile(StoryScene scene)
    {
        currentScene = scene;
        backGroundController.SetImage(currentScene.background);
        _lastBackground = currentScene.background;

        TextAsset csvData = currentScene.csvFile;

        string[] data = csvData.text.Split(new char[] { '\n' });

        int lastSentenceIndex = -1;
        _lastSpeaker = null;
        _lastBackground = null;

        currentScene.sentences = new List<StoryScene.Sentence>();
        int cnt = 0;

        currentScene.summaryText = data[data.Length - 2].Split(new char[] {','})[0];
        Debug.Log(data[data.Length - 1].Split(new char[] {','})[0]);

        for (int i = 1; i < data.Length - 2; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            
            StoryScene.Sentence dialogue = new StoryScene.Sentence();

            if (row[_CSV_SPEAKER_INDEX] != "")
            {
                _lastSpeaker = Resources.Load<Speaker>("Speaker/" + row[_CSV_SPEAKER_INDEX].Trim());
            }
            dialogue.speaker = _lastSpeaker;

            if (row[_CSV_TEXT_INDEX] != "")
            {
                ++lastSentenceIndex;
                dialogue.text = row[_CSV_TEXT_INDEX];
            }

            if (row[_CSV_BACKGROUND_INDEX] != "")
            {
                _lastBackground = Resources.Load<Sprite>("Background/" + row[_CSV_BACKGROUND_INDEX].Trim());
            }
            dialogue.background = _lastBackground;

            if (row[_CSV_AUDIOCLIP_INDEX] != "")
            {
                dialogue.audioClip = Resources.Load<AudioClip>("AudioClips/" + row[_CSV_AUDIOCLIP_INDEX].Trim());
            }

            dialogue.nextSentenceDelay = (row[_CSV_NEXTDELAY_INDEX] != "" ? float.Parse(row[_CSV_NEXTDELAY_INDEX]) : 0);
            
            List<StoryScene.Sentence.Action> actionList = new List<StoryScene.Sentence.Action>();
            StoryScene.Sentence.Action action = new StoryScene.Sentence.Action();
            if (row[_CSV_ACTION_INDEX] != "")
            {
                Debug.Log("cnt : " + ++cnt);
                action.speaker = Resources.Load<Speaker>("Speaker/" + row[_CSV_ACTION_INDEX].Trim());

                int xPos = 0;
                int yPos = 0;
                
                switch (row[_CSV_ACTION_TYPE_INDEX])
                {
                    case "Appear":
                        action.actionType = StoryScene.Sentence.Action.Type.Appear;
                        xPos = int.Parse(row[_CSV_ACTION_COORDS_X_INDEX]);
                        yPos = int.Parse(row[_CSV_ACTION_COORDS_Y_INDEX]);
                        action.coords = new Vector2(xPos, yPos);
                        action.spriteIndex = int.Parse(row[_CSV_ACTION_SPRITEINDEX_INDEX]);
                        break;
                    
                    case "Move":
                        action.actionType = StoryScene.Sentence.Action.Type.Move;
                        xPos = int.Parse(row[_CSV_ACTION_COORDS_X_INDEX]);
                        yPos = int.Parse(row[_CSV_ACTION_COORDS_Y_INDEX]);
                        action.coords = new Vector2(xPos, yPos);
                        action.moveSpeed = float.Parse(row[_CSV_ACTION_MOVESPEED_INDEX]);
                        break;
                    
                    case "Change":
                        action.actionType = StoryScene.Sentence.Action.Type.Change;
                        action.spriteIndex = int.Parse(row[_CSV_ACTION_SPRITEINDEX_INDEX]);
                        break;
                    
                    case "DisAppear":
                        action.actionType = StoryScene.Sentence.Action.Type.DisAppear;
                        break;
                }
                
            }
            
            if (row[_CSV_TEXT_INDEX].Trim() == "" && row[_CSV_ACTION_INDEX] != "")
            {
                Debug.Log(action.actionType);
                Debug.Log(lastSentenceIndex);
                currentScene.sentences[lastSentenceIndex].actions.Add(action);
            }
            else
            {
                if (row[_CSV_ACTION_INDEX] != "")
                {
                    Debug.Log(row[_CSV_ACTION_INDEX]);
                    actionList.Add(action);
                    dialogue.actions = actionList;
                }
                currentScene.sentences.Add(dialogue);
            }
        }
    }

    IEnumerator DelayNextSentence(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isDelayFinish = true;
    }

    public bool IsCompleted()
    {
        return _isDelayFinish && _state == State.Completed;
    }

    public bool IsLastSentence()
    {
        return _sentenceIndex + 1 ==  currentScene.sentences.Count;
    }

    public void PlayNextSentence()
    {
        _isDelayFinish = false;
        
        StartCoroutine(TypeText
            (currentScene.sentences[++_sentenceIndex].text));
        
        StartCoroutine(DelayNextSentence(currentScene.sentences[_sentenceIndex].nextSentenceDelay));
        
        speakerNameText.text = currentScene.sentences[_sentenceIndex].speaker.speakerName;
        speakerNameText.color = currentScene.sentences[_sentenceIndex].speaker.nameColor;

        if (currentScene.sentences[_sentenceIndex].background != _lastBackground)
        {
            backGroundController.SwitchImage(currentScene.sentences[_sentenceIndex].background);
            _lastBackground = currentScene.sentences[_sentenceIndex].background;
        }
        
        _audioSource.clip = currentScene.sentences[_sentenceIndex].audioClip;
        _audioSource.Play();

        ActSpeakers();
    }
    
    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        text = text.Replace("`", ",");
        _state = State.Playing;
        int wordIndex = 0;

        while (_state != State.Completed)
        {
            dialogueText.text += text[wordIndex];
            yield return new WaitForSeconds(typeDelay);

            if (++wordIndex >= text.Length)
            {
                _state = State.Completed;
                break;
            }
        }
    }

    void ActSpeakers()
    {
        List<StoryScene.Sentence.Action> actions
            = currentScene.sentences[_sentenceIndex].actions;
        
        for (int i = 0; i < actions?.Count; i++)
        {
            ActSpeaker(actions[i]);
        }
    }

    void ActSpeaker(StoryScene.Sentence.Action action)
    {
        SpriteController controller = null;

        switch (action.actionType)
        {
            case StoryScene.Sentence.Action.Type.Appear:
                if (!_sprites.ContainsKey(action.speaker))
                {
                    controller = Instantiate(action.speaker.prefab.gameObject, spritesPrefab.transform)
                            .GetComponent<SpriteController>();
                    
                    _sprites.Add(action.speaker, controller);
                }
                else
                {
                    controller = _sprites[action.speaker];
                }
                controller.SetUp(action.speaker.sprites[action.spriteIndex]);
                controller.Show(action.coords);
                return;
            
            case StoryScene.Sentence.Action.Type.Move:
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.Move(action.coords, action.moveSpeed);
                }
                break;
            
            case StoryScene.Sentence.Action.Type.Change:
                controller = _sprites[action.speaker];
                controller.SwitchSprite(action.speaker.sprites[action.spriteIndex]);
                break;
            
            case StoryScene.Sentence.Action.Type.DisAppear:
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.Hide();
                }
                break;
        }
    }

    public void EndScene()
    {
        _sprites.Clear();
        StopAllCoroutines();

        foreach (Transform child in spritesPrefab.transform)
        {
            Destroy(child.gameObject);
        }
    }
}