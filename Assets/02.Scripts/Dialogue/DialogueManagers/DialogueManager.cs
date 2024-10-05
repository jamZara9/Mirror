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

public class DialogueManager : Singleton<DialogueManager>, IManager
{
    private CsvParser csvParser;
    
    // VN 방식 UI. 
    [Header ("Visual Novel Dialogue Object")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    
    // Movable 방식 UI 패널 및 화자 이름 및 대사 텍스트.
    [Header ("Movable Dialogue Object")]
    [SerializeField] private GameObject movableDialoguePanel;
    [SerializeField] private TextMeshProUGUI movableSpeakerNameText;
    [SerializeField] private TextMeshProUGUI sentenceText;


    private int _sentenceIndex = -1;
    public StoryScene currentScene;

    [SerializeField] private BackGroundController backGroundController;

    [Header ("Dialogue Parameters")]
    // 대사 텍스트 입력 딜레이.
    [SerializeField] private float typeDelay = 0.025f;
    
    // 다음 문장으로 넘어가는 기본 딜레이 시간.
    [SerializeField] private float defaultDelay = 1.25f;

    // 스프라이트별 오브젝트 및 컨트롤러 저장
    private Dictionary<Speaker, SpriteController> _sprites;
    public GameObject spritesPrefab;

    // 메인 BGM 및 사운드 이펙트 AudioSource.
    [Header ("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    public AudioSource GetBGMAudoiSource(){
        return audioSource;
    }

    private bool _isDelayFinish = true;

    #region  Dialogue Scene Active Test Code
    [Header ("Dialogue Scene Active Test Parameters")]
    private bool _isDialogueSceneActive = false;
    public bool IsDialogueSceneActive {
        get => _isDialogueSceneActive;
        set {
            if(_isDialogueSceneActive != value)
            {
                _isDialogueSceneActive = value;
                HandleDialogueSceneActive();
            }
        }
    }

    private void HandleDialogueSceneActive(){
        if(IsDialogueSceneActive)
        {
            // InputManager.Instance.SwitchActionMap("Dialogue");
            GameManager.inputManager.SwitchActionMap("Dialogue");
        }
        else
        {
            // InputManager.Instance.SwitchActionMap("Player");
            GameManager.inputManager.SwitchActionMap("Player");
        }
    }

    public GameObject visualNovelDialoguePanel;               // 비주얼 노벨 대화 패널
    public GameObject skipPanel;                              // 스킵 패널
    public TextMeshProUGUI skipPanelStoryText;                // 스킵 패널 스토리 텍스트

    /// <summary>
    /// 조건에 맞게 다이얼로그 매니저의 스토리 실행 함수를 호출해 스토리를 시작시키는 함수  
    /// 기존 GameController.PlayScene() -> DialogueManager.StartStoryScene()로 변경
    /// </summary>
    /// <param name="storyScene">다이얼로그 매니저에 넘겨 실행할 스토리</param>
    public void StartStoryScene(StoryScene storyScene)
    {
        currentScene = storyScene;

        switch (storyScene.storyType)
        {
            case StoryScene.StoryType.VisualNovel :
                visualNovelDialoguePanel.SetActive(true);
                IsDialogueSceneActive = true;
                break;
            
            case StoryScene.StoryType.Movable :
                movableDialoguePanel.SetActive(true);
                break;
        }
        PlayScene(storyScene, storyScene.storyType);
    }

    #endregion


    // 타이핑 상태 열거.
    private enum State
    {
        // 타이핑 중, 타이핑 끝.
        Playing, Completed
    }
    private State _state = State.Completed;

    // IManager 인터페이스 구현.
    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.PlaygroundA)
        {
            // CSV Parser 객체 생성.
            csvParser = new();

            // 딕셔너리 초기화.
            _sprites = new Dictionary<Speaker, SpriteController>();
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
    /// 현재 문장이 해당 스토리의 마지막 문장인지 확인하는 함수.
    /// </summary>
    public bool IsLastSentence()
    {
        return _sentenceIndex + 1 ==  currentScene.sentences.Count;
    }
    
    /// <summary>
    /// 한 스토리를 시작하는 함수.
    /// </summary>
    public void PlayScene(StoryScene scene, StoryScene.StoryType storyType)
    {
        // 스토리 파싱하여 데이터 저장.
        currentScene = csvParser.ParseCSVFile(scene);
        
        // 문장 인덱스 초기화.
        _sentenceIndex = -1;
        
        // 스토리 BGM 실행.
        GameManager.audioManager.PlayBackgroundMusic(currentScene.backgroundMusic, 1.0f);

        // 스토리 타입에 따라 문장 실행.
        switch (storyType)
        {
            // 비주얼 노벨 방식 진행.
            case StoryScene.StoryType.VisualNovel :
                VisualNovelNextSentence();
                break;
            
            //움직일 수 있는 방식 실행.
            case StoryScene.StoryType.Movable :
                StartCoroutine(MovableNextSentence());
                break;
            
            // 간단한 정보, 알림 패널 방식 실행. 
            case StoryScene.StoryType.Information :
                break;
        }
    }
    
    /// <summary>
    /// 비주얼 노벨 방식의 다음 문장을 실행시키는 함수.
    /// </summary>
    public void VisualNovelNextSentence()
    {
        _isDelayFinish = false;
        
        StartCoroutine(TypeText(currentScene.sentences[++_sentenceIndex].text));
        
        StartCoroutine(DelayNextSentence(currentScene.sentences[_sentenceIndex].nextSentenceDelay));
        
        speakerNameText.text = currentScene.sentences[_sentenceIndex].speaker.speakerName;
        speakerNameText.color = currentScene.sentences[_sentenceIndex].speaker.nameColor;
        
        backGroundController.SwitchImage(currentScene.sentences[_sentenceIndex].background);
    
        // sentenceAudioSource.clip = currentScene.sentences[_sentenceIndex].audioClip;
        // sentenceAudioSource.Play();
        // AudioManager.Instance.PlaySoundEffect(currentScene.sentences[_sentenceIndex].audioClip, transform.position, 1.0f);

        GameManager.audioManager.PlaySoundEffect(currentScene.sentences[_sentenceIndex].audioClip, transform.position, 1.0f);

        ActSpeakers();
    }
    
    /// <summary>
    /// 움직일 수 있는 방식의 다음 문장으로 넘어가는 로직 처리 함수.
    /// </summary>
    /// <returns></returns>
    IEnumerator MovableNextSentence()
    {
        // 화자 이름 텍스트 UI를 해당 순서 문장의 화자 이름 및 색상으로 변경.
        movableSpeakerNameText.text = currentScene.sentences[++_sentenceIndex].speaker.speakerName;
        movableSpeakerNameText.color = currentScene.sentences[_sentenceIndex].speaker.nameColor;

        // 대사 텍스트 UI를 해당 순서 문장의 대사로 변경.
        sentenceText.text = currentScene.sentences[_sentenceIndex].text;
        
        // 해당 문장의 딜레이 시간만큼 다음 문장으로 넘어가지 않고 대기.
        float delay = currentScene.sentences[_sentenceIndex].nextSentenceDelay;
        // 딜레이를 지정하지 않았다면 기본 딜레이 시간으로 세팅.
        if (delay == 0)
        {
            delay = defaultDelay;
        }
        yield return new WaitForSeconds(delay);

        // 다음 문장으로 넘어가기 위해 인덱스값 증가.
        // 마지막 문장이 아니면 다음 문장 호출.
        if (_sentenceIndex + 1 < currentScene.sentences.Count)
        {
            StartCoroutine(MovableNextSentence());
        }
        // 마지막 문장이 맞으면 끝내기.
        else
        {
            EndScene();
        }
    }
    
    /// <summary>
    /// 대사를 한 글자씩 입력하는 함수.
    /// </summary>
    /// <param name="text">CSV의 대사 문자열</param>
    IEnumerator TypeText(string text)
    {
        // 타이핑 전 텍스트 초기화.
        dialogueText.text = "";
        // 현재 상태를 타이핑 중으로 변경.
        _state = State.Playing;
        // 현재 글자 인덱스 초기화.
        int wordIndex = 0;

        // 모든 글자를 입력하기 전까지 타이핑.
        while (_state != State.Completed)
        {
            // 텍스트에 글자 하나씩 추가.
            dialogueText.text += text[wordIndex];
            // 타이핑 딜레이만큼 대기.
            yield return new WaitForSeconds(typeDelay);

            // 모든 글자를 입력했으면 상태를 완료로 변경.
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
            if (currentSpeaker != sprite.Key)
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
        // 딕셔너리 초기화.
        _sprites.Clear();
        // 모든 코루틴 정지.
        StopAllCoroutines();
        audioSource.Stop();
        
        // UI 패널 비활성화.
        movableDialoguePanel.SetActive(false);

        // 생성한 캐릭터 스프라이트 모두 삭제. 
        foreach (Transform child in spritesPrefab.transform)
        {
            Destroy(child.gameObject);
        }

        // Test
        IsDialogueSceneActive = false;
    }
}