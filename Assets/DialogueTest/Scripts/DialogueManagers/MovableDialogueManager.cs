using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovableDialogueManager : MonoBehaviour
{
    // 현재 플레이하는 스토리.
    private StoryScene _currentMovableStoryScene;

    // CSV 작성 편의를 위해 마지막 화자를 저장하는 변수.
    private Speaker _lastSpeaker = null;

    // UI 패널에 들어갈 화자 이름 및 대사 텍스트.
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI sentenceText;

    // 다음 문장으로 넘어가는 기본 딜레이 시간.
    [SerializeField] private float defaultDelay = 1.25f;
    
    // 현재 읽고 있는 문장 인덱스.
    private int _sentenceIndex = 0;
    
    // CSV 파일의 항목별 인덱스 값 저장.
    private const int _CSV_SPEAKER_INDEX = 0;
    private const int _CSV_TEXT_INDEX = 1;
    private const int _CSV_NEXTDELAY_INDEX = 4;
    
    /// <summary>
    /// 한 스토리를 시작하는 함수.
    /// </summary>
    public void PlayScene()
    {
        // 문장 인덱스 초기화.
        _sentenceIndex = 0;

        // 문장 읽기 시작.
        StartCoroutine(PlayNextSentence());
    }
    
    /// <summary>
    /// 각 스토리의 CSV 파일에 맞게 데이터를 파싱하는 함수. 
    /// </summary>
    /// /// <param name="scene">파싱할 StoryScene 스크립터블 오브젝트</param>
    public void ParseCSVFile(StoryScene scene)
    {
        // 플레이를 시작할 씬을 받아 저장.
        _currentMovableStoryScene = scene;
        
        // CSV 파일을 읽기 위해 저장.
        TextAsset csvData = _currentMovableStoryScene.csvFile;
        // CSV 파일 내, 각 행을 분리하여 배열로 저장.
        string[] data = csvData.text.Split(new char[] { '\n' });
        
        // 마지막 화자를 저장하는 변수 초기화.
        _lastSpeaker = null;
        
        // 파싱할 현재 스토리의 문장 리스트 초기화.
        _currentMovableStoryScene.sentences = new List<StoryScene.Sentence>();

        // CSV 파일의 각 행을 돌며 처리 시작.
        for (int i = 1; i < data.Length - 1; i++)
        {
            // 각 열을 분리하여 배열로 저장. 
            string[] row = data[i].Split(new char[] { ',' });
            
            // 임시로 파싱한 데이터을 저장할 변수 초기화. 
            StoryScene.Sentence dialogue = new StoryScene.Sentence();
            
            // 문장의 화자 세팅.
            // 만약 문장의 화자 열이 비어있지 않다면 마지막 화자에 저장.
            if (row[_CSV_SPEAKER_INDEX] != "")
            {
                _lastSpeaker = Resources.Load<Speaker>("Speaker/" + row[_CSV_SPEAKER_INDEX].Trim());
            }
            // 문장 화자에 마지막 화자를 지정. 
            dialogue.speaker = _lastSpeaker;

            // 대사가 빈 문장이 아닐 때의 처리.
            if (row[_CSV_TEXT_INDEX] != "")
            {
                // " ` " 문자를 " , " 문자로 변경 (CSV 파일이 ,를 기준으로 처리를 하기 때문에 , 대신 `를 사용하여야 함.
                dialogue.text = row[_CSV_TEXT_INDEX].Replace("`", ",");
            }
            
            // 딜레이 열에 값이 지정돼 있다면 해당 값 저장, 아니라면 기본 값 저장.
            dialogue.nextSentenceDelay = (row[_CSV_NEXTDELAY_INDEX].Trim() != "" ? float.Parse(row[_CSV_NEXTDELAY_INDEX]) : defaultDelay);
            
            // 임시 저장한 문장 정보를 실제 사용하는 오브젝트의 정보로 저장.
            _currentMovableStoryScene.sentences.Add(dialogue);
        }
    }
    
    /// <summary>
    /// 다음 문장으로 넘어가는 로직 처리 함수.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayNextSentence()
    {
        // 화자 이름 텍스트 UI를 해당 순서 문장의 화자 이름 및 색상으로 변경.
        speakerNameText.text = _currentMovableStoryScene.sentences[_sentenceIndex].speaker.speakerName;
        speakerNameText.color = _currentMovableStoryScene.sentences[_sentenceIndex].speaker.nameColor;

        // 대사 텍스트 UI를 해당 순서 문장의 대사로 변경.
        sentenceText.text = _currentMovableStoryScene.sentences[_sentenceIndex].text;
        
        // 해당 문장의 딜레이 시간만큼 다음 문장으로 넘어가지 않고 대기.
        yield return new WaitForSeconds(_currentMovableStoryScene.sentences[_sentenceIndex].nextSentenceDelay);

        // 다음 문장으로 넘어가기 위해 인덱스값 증가.
        // 마지막 문장이 아니면 다음 문장 호출.
        if (++_sentenceIndex < _currentMovableStoryScene.sentences.Count)
        {
            StartCoroutine(PlayNextSentence());
        }
        // 마지막 문장이 맞으면 끝내기.
        else
        {
            EndScene();
        }
    }

    /// <summary>
    ///  한 스토리가 끝났을 때의 처리를 담당하는 함수.
    /// </summary>
    void EndScene()
    {
        // 모든 코루틴 멈춤.
        StopAllCoroutines();

        // UI 패널 비활성화.
        gameObject.SetActive(false);
    }
}
