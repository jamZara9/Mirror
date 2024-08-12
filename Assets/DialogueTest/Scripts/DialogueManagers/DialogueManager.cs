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
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;

    private int _sentenceIndex = -1;
    public StoryScene currentScene;

    [SerializeField] private BackGroundController backGroundController;

    public float typeDelay = 0.05f;

    // 스프라이트별 오브젝트 및 컨트롤러 저장
    private Dictionary<Speaker, SpriteController> _sprites;
    public GameObject spritesPrefab;

    // 메인 BGM 및 사운드 이펙트 AudioSource.
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sentenceAudioSource;

    [SerializeField] private Speaker narrationSpeaker;

    private bool _isDelayFinish = true;

    // CSV 파일 작성 편의를 위한 임시 저장 변수.
    private Speaker _lastSpeaker = null;
    private Sprite _lastBackground = null;

    // CSV 파일의 항목별 인덱스 값 저장.
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
    private const int _CSV_ACTION_SCALE_WIDTH_INDEX = 11;
    private const int _CSV_ACTION_SCALE_HEIGHT_INDEX = 12;
    private const int _CSV_ACTION_SCALE_SPEED_INDEX = 13;

    // 타이핑 상태 열거.
    private enum State
    {
        // 타이핑 중, 타이핑 끝.
        Playing, Completed
    }
    private State _state = State.Completed;
    
    void Awake()
    {
        // 딕셔너리 초기화.
        _sprites = new Dictionary<Speaker, SpriteController>();
    }

    /// <summary>
    /// 한 스토리를 시작하는 함수.
    /// </summary>
    public void PlayScene()
    {
        _sentenceIndex = -1;
        
        audioSource.clip = currentScene.backgroundMusic;
        audioSource.Play();

        PlayNextSentence();
    }

    /// <summary>
    /// 각 스토리의 CSV 파일에 맞게 데이터를 파싱하는 함수. 
    /// </summary>
    /// <param name="scene">파싱할 StoryScene 스크립터블 오브젝트</param>
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
                dialogue.text = row[_CSV_TEXT_INDEX] == "null" ? " " : row[_CSV_TEXT_INDEX];
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
                action.speaker = Resources.Load<Speaker>("Speaker/" + row[_CSV_ACTION_INDEX].Trim());

                int xPos = 0;
                int yPos = 0;
                
                switch (row[_CSV_ACTION_TYPE_INDEX])
                {
                    case "Appear" :
                        action.actionType = StoryScene.Sentence.Action.Type.Appear;
                        xPos = int.Parse(row[_CSV_ACTION_COORDS_X_INDEX]);
                        yPos = int.Parse(row[_CSV_ACTION_COORDS_Y_INDEX]);
                        action.coords = new Vector2(xPos, yPos);
                        action.spriteIndex = int.Parse(row[_CSV_ACTION_SPRITEINDEX_INDEX]);
                        break;
                    
                    case "First" :
                        action.actionType = StoryScene.Sentence.Action.Type.First;
                        break;
                    
                    case "Bright" :
                        action.actionType = StoryScene.Sentence.Action.Type.Bright;
                        break;
                    
                    case "AllDark" :
                        action.actionType = StoryScene.Sentence.Action.Type.AllDark;
                        break;
                    
                    case "AllBright" :
                        action.actionType = StoryScene.Sentence.Action.Type.AllBright;
                        break;

                    case "Move" :
                        action.actionType = StoryScene.Sentence.Action.Type.Move;
                        xPos = int.Parse(row[_CSV_ACTION_COORDS_X_INDEX]);
                        yPos = int.Parse(row[_CSV_ACTION_COORDS_Y_INDEX]);
                        action.coords = new Vector2(xPos, yPos);
                        action.moveSpeed = float.Parse(row[_CSV_ACTION_MOVESPEED_INDEX]);
                        break;
                    
                    case "Change" :
                        action.actionType = StoryScene.Sentence.Action.Type.Change;
                        action.spriteIndex = int.Parse(row[_CSV_ACTION_SPRITEINDEX_INDEX]);
                        break;
                    
                    case "Scale" :
                        action.actionType = StoryScene.Sentence.Action.Type.Scale;
                        action.width = int.Parse(row[_CSV_ACTION_SCALE_WIDTH_INDEX]);
                        action.height = int.Parse(row[_CSV_ACTION_SCALE_HEIGHT_INDEX]);
                        action.scaleSpeed = float.Parse(row[_CSV_ACTION_SCALE_SPEED_INDEX]);
                        break;
                    
                    case "DisAppear" :
                        action.actionType = StoryScene.Sentence.Action.Type.DisAppear;
                        break;
                }
            }
            
            if (row[_CSV_TEXT_INDEX].Trim() == "" && row[_CSV_ACTION_INDEX] != "")
            {
                currentScene.sentences[lastSentenceIndex].actions.Add(action);
            }
            else
            {
                if (row[_CSV_ACTION_INDEX] != "")
                {
                    actionList.Add(action);
                    dialogue.actions = actionList;
                }
                currentScene.sentences.Add(dialogue);
            }
        }
    }

    /// <summary>
    /// 다음 문장으로 넘어가기 위한 딜레이를 시작하는 함수.
    /// </summary>
    /// <param name="delay">문장별 딜레이 시간</param>
    IEnumerator DelayNextSentence(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isDelayFinish = true;
    }

    /// <summary>
    /// 다음 문장으로 넘어갈 준비가 된 상태 여부를 반환하는 함수 
    /// </summary>
    public bool IsCompleted()
    {
        return _isDelayFinish && _state == State.Completed;
    }

    
    /// <summary>
    /// 현재 문장이 해당 스토리의 마지막 문장인지 참거짓을 반환하는 함수.
    /// </summary>
    public bool IsLastSentence()
    {
        return _sentenceIndex + 1 ==  currentScene.sentences.Count;
    }
    
    /// <summary>
    /// 다음 문장을 실행시키는 함수.
    /// </summary>
    public void PlayNextSentence()
    {
        _isDelayFinish = false;
        
        StartCoroutine(TypeText(currentScene.sentences[++_sentenceIndex].text));
        
        StartCoroutine(DelayNextSentence(currentScene.sentences[_sentenceIndex].nextSentenceDelay));
        
        speakerNameText.text = currentScene.sentences[_sentenceIndex].speaker.speakerName;
        speakerNameText.color = currentScene.sentences[_sentenceIndex].speaker.nameColor;

        if (currentScene.sentences[_sentenceIndex].background != _lastBackground)
        {
            backGroundController.SwitchImage(currentScene.sentences[_sentenceIndex].background);
            _lastBackground = currentScene.sentences[_sentenceIndex].background;
        }
        
        sentenceAudioSource.clip = currentScene.sentences[_sentenceIndex].audioClip;
        sentenceAudioSource.Play();

        ActSpeakers();
    }
    
    /// <summary>
    /// 대사를 한 글자씩 입력하는 함수.
    /// </summary>
    /// <param name="text">CSV의 대사 문자열</param>
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

    /// <summary>
    /// 각 문장별 액션 리스트를 처리하는 함수.
    /// </summary>
    void ActSpeakers()
    {
        Speaker currentSpeaker = currentScene.sentences[_sentenceIndex].speaker;
        
        // 기본 연출.
        // 현재 문장을 말하는 스프라이트 밝게, 나머지 모든 스프라이트 어둡게 함. 
        foreach (var sprite in _sprites)
        {
            if (currentSpeaker != sprite.Key && currentSpeaker != narrationSpeaker)
            {
                sprite.Value.SetColorDark();
            }
            else
            {
                sprite.Value.SetFirst();
                sprite.Value.SetColorOrigin();
            }
        }
        
        List<StoryScene.Sentence.Action> actions = currentScene.sentences[_sentenceIndex].actions;
        
        for (int i = 0; i < actions?.Count; i++)
        {
            ActSpeaker(actions[i]);
        }
    }

    /// <summary>
    /// 각 스프라이트의 액션을 실행시키는 함수.
    /// </summary>
    /// <param name="action">StoryScene.Sentence.Action 타입</param>
    void ActSpeaker(StoryScene.Sentence.Action action)
    {
        SpriteController controller = null;

        switch (action.actionType)
        {
            case StoryScene.Sentence.Action.Type.Appear :
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
                break;
            
            case StoryScene.Sentence.Action.Type.First :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.SetFirst();
                }
                break;
            
            case StoryScene.Sentence.Action.Type.AllDark :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.AllDark(_sprites);
                }
                break;
            
            case StoryScene.Sentence.Action.Type.AllBright :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.AllBright(_sprites);
                }
                break;

            case StoryScene.Sentence.Action.Type.Bright :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.SetColorOrigin();
                }
                break;
            
            case StoryScene.Sentence.Action.Type.Move :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.Move(action.coords, action.moveSpeed);
                }
                break;
            
            case StoryScene.Sentence.Action.Type.Change :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.SwitchSprite(action.speaker.sprites[action.spriteIndex]);
                }
                break;
            
            case StoryScene.Sentence.Action.Type.Scale :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.Scale(action.width, action.height, action.scaleSpeed);
                }
                break;
            
            case StoryScene.Sentence.Action.Type.DisAppear :
                if (_sprites.ContainsKey(action.speaker))
                {
                    controller = _sprites[action.speaker];
                    controller.Hide();
                }
                break;
        }
    }

    /// <summary>
    ///  한 스토리가 끝났을 때의 처리를 담당하는 함수.
    /// </summary>
    public void EndScene()
    {
        // 필수 스토리면 스토리 이벤트 시작.
        if (currentScene.isMainStory)
        {
            StoryManager.Instance.PlayStoryEvent();
        }
        _sprites.Clear();
        StopAllCoroutines();

        foreach (Transform child in spritesPrefab.transform)
        {
            Destroy(child.gameObject);
        }
    }
}