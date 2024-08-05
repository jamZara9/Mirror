using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovableDialogueManager : MonoBehaviour
{
    private StoryScene _currentMovableStoryScene;

    private Speaker _lastSpeaker = null;

    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI sentenceText;

    // 다음 문장으로 넘어가는 기본 딜레이 시간.
    [SerializeField] private float defaultDelay = 1.25f;
    
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
        _sentenceIndex = 0;

        StartCoroutine(PlayNextSentence());
    }
    
    /// <summary>
    /// 각 스토리의 CSV 파일에 맞게 데이터를 파싱하는 함수. 
    /// </summary>
    /// /// <param name="scene">파싱할 StoryScene 스크립터블 오브젝트</param>
    public void ParseCSVFile(StoryScene scene)
    {
        _currentMovableStoryScene = scene;
        
        TextAsset csvData = _currentMovableStoryScene.csvFile;
        string[] data = csvData.text.Split(new char[] { '\n' });
        
        _lastSpeaker = null;
        
        _currentMovableStoryScene.sentences = new List<StoryScene.Sentence>();

        for (int i = 1; i < data.Length - 1; i++)
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
                dialogue.text = row[_CSV_TEXT_INDEX].Replace("`", ",");
            }
            
            dialogue.nextSentenceDelay = (row[_CSV_NEXTDELAY_INDEX].Trim() != "" ? float.Parse(row[_CSV_NEXTDELAY_INDEX]) : defaultDelay);
            
            _currentMovableStoryScene.sentences.Add(dialogue);
        }
    }
    
    /// <summary>
    /// 다음 문장으로 넘어가는 로직 처리 함수.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayNextSentence()
    {
        speakerNameText.text = _currentMovableStoryScene.sentences[_sentenceIndex].speaker.speakerName;
        speakerNameText.color = _currentMovableStoryScene.sentences[_sentenceIndex].speaker.nameColor;

        sentenceText.text = _currentMovableStoryScene.sentences[_sentenceIndex].text;
        
        yield return new WaitForSeconds(_currentMovableStoryScene.sentences[_sentenceIndex].nextSentenceDelay);

        if (++_sentenceIndex < _currentMovableStoryScene.sentences.Count)
        {
            StartCoroutine(PlayNextSentence());
        }
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
        StopAllCoroutines();

        gameObject.SetActive(false);
    }
}
