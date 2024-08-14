using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CsvParser : MonoBehaviour
{
    // 파싱 후 반환할 데이터를 저장하는 변수.
    private StoryScene _sceneData;
    
    // CSV 파일 작성 편의를 위한 임시 저장 변수.
    private Speaker _lastSpeaker = null;

    private string _sheetData;

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
    
    IEnumerator SetSheetData(string URL)
    {
        // UnityWebRequest 인스턴스 리소스 해제를 위해 Using 사용함.
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                _sheetData = www.downloadHandler.text;
            }
        }
    }

    /// <summary>
    /// 각 스토리의 CSV 파일에 맞게 데이터를 파싱하는 함수. 
    /// </summary>
    /// <param name="scene">파싱할 StoryScene 스크립터블 오브젝트</param>
    public StoryScene ParseCSVFile(StoryScene scene)
    {
        _sceneData = scene;

        TextAsset csvData = _sceneData.csvFile;

        string[] data = csvData.text.Split(new char[] { '\n' });

        int lastSentenceIndex = -1;
        _lastSpeaker = null;

        _sceneData.sentences = new List<StoryScene.Sentence>();

        _sceneData.summaryText = data[data.Length - 2].Split(new char[] {','})[0];

        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            if (row.Length <= 1)
            {
                break;
            }
            
            StoryScene.Sentence dialogue = new StoryScene.Sentence();

            if (row[_CSV_SPEAKER_INDEX] != "")
            {
                _lastSpeaker = Resources.Load<Speaker>("Speaker/" + row[_CSV_SPEAKER_INDEX].Trim());
            }
            dialogue.speaker = _lastSpeaker;

            if (row[_CSV_TEXT_INDEX] != "")
            {
                ++lastSentenceIndex;
                dialogue.text = row[_CSV_TEXT_INDEX] == "null" ? " " : row[_CSV_TEXT_INDEX].Replace("`", ",");
            }

            if (row[_CSV_BACKGROUND_INDEX] != "")
            {
                dialogue.background = Resources.Load<Sprite>("Background/" + row[_CSV_BACKGROUND_INDEX].Trim());
            }

            if (row[_CSV_AUDIOCLIP_INDEX] != "")
            {
                dialogue.audioClip = Resources.Load<AudioClip>("AudioClips/" + row[_CSV_AUDIOCLIP_INDEX].Trim());
            }
            
            dialogue.nextSentenceDelay = (row[_CSV_NEXTDELAY_INDEX].Trim() != "" ? float.Parse(row[_CSV_NEXTDELAY_INDEX]) : 0.0f);

            if (row.Length <= _CSV_ACTION_INDEX)
            {
                _sceneData.sentences.Add(dialogue);
                continue;
            }
            
            List<StoryScene.Sentence.Action> actionList = new List<StoryScene.Sentence.Action>();
            StoryScene.Sentence.Action action = ActionParse(row);
            
            if (row[_CSV_TEXT_INDEX].Trim() == "" && row[_CSV_ACTION_INDEX] != "")
            {
                _sceneData.sentences[lastSentenceIndex].actions.Add(action);
            }
            else
            {
                if (row[_CSV_ACTION_INDEX] != "")
                {
                    actionList.Add(action);
                    dialogue.actions = actionList;
                }
                _sceneData.sentences.Add(dialogue);
            }
        }
        
        return _sceneData;
    }

    private StoryScene.Sentence.Action ActionParse(string[] row)
    {
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

        return action;
    }
    
    // /// <summary>
    // /// 각 스토리의 CSV 파일에 맞게 데이터를 파싱하는 함수. 
    // /// </summary>
    // /// /// <param name="scene">파싱할 StoryScene 스크립터블 오브젝트</param>
    // public StoryScene ParseMovableCSVFile(StoryScene scene)
    // {
    //     // 플레이를 시작할 씬을 받아 저장.
    //     _sceneData = scene;
    //     
    //     // CSV 파일을 읽기 위해 저장.
    //     TextAsset csvData = _sceneData.csvFile;
    //     // CSV 파일 내, 각 행을 분리하여 배열로 저장.
    //     string[] data = csvData.text.Split(new char[] { '\n' });
    //     
    //     // 마지막 화자를 저장하는 변수 초기화.
    //     _lastSpeaker = null;
    //     
    //     // 파싱할 현재 스토리의 문장 리스트 초기화.
    //     _sceneData.sentences = new List<StoryScene.Sentence>();
    //
    //     // CSV 파일의 각 행을 돌며 처리 시작.
    //     for (int i = 1; i < data.Length - 1; i++)
    //     {
    //         // 각 열을 분리하여 배열로 저장. 
    //         string[] row = data[i].Split(new char[] { ',' });
    //         
    //         // 임시로 파싱한 데이터을 저장할 변수 초기화. 
    //         StoryScene.Sentence dialogue = new StoryScene.Sentence();
    //         
    //         // 문장의 화자 세팅.
    //         // 만약 문장의 화자 열이 비어있지 않다면 마지막 화자에 저장.
    //         if (row[_CSV_SPEAKER_INDEX] != "")
    //         {
    //             _lastSpeaker = Resources.Load<Speaker>("Speaker/" + row[_CSV_SPEAKER_INDEX].Trim());
    //         }
    //         // 문장 화자에 마지막 화자를 지정. 
    //         dialogue.speaker = _lastSpeaker;
    //
    //         // 대사가 빈 문장이 아닐 때의 처리.
    //         if (row[_CSV_TEXT_INDEX] != "")
    //         {
    //             // " ` " 문자를 " , " 문자로 변경 (CSV 파일이 ,를 기준으로 처리를 하기 때문에 , 대신 `를 사용하여야 함.
    //             dialogue.text = row[_CSV_TEXT_INDEX].Replace("`", ",");
    //         }
    //         
    //         // 딜레이 열에 값이 지정돼 있다면 해당 값 저장, 아니라면 기본 값 저장.
    //         dialogue.nextSentenceDelay = (row[_CSV_NEXTDELAY_INDEX].Trim() != "" ?
    //             float.Parse(row[_CSV_NEXTDELAY_INDEX]) : defaultDelay);
    //         
    //         // 임시 저장한 문장 정보를 실제 사용하는 오브젝트의 정보로 저장.
    //         _sceneData.sentences.Add(dialogue);
    //     }
    //
    //     return _sceneData;
    // }
}
