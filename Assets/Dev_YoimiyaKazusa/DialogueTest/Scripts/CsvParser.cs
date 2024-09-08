using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CsvParser
{
    // 파싱 후 반환할 데이터를 저장하는 변수.
    private StoryScene _sceneData;
    
    // CSV 파일 작성 편의를 위한 임시 저장 변수.
    private Speaker _lastSpeaker = null;

    // CSV의 Speaker 이름 문자열을 받아 speaker 스트립터블을 저장 및 사용하는 딕셔너리.
    private Dictionary<string, Speaker> _speakerDictionary = new Dictionary<string, Speaker>();

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

            // @Todo: 임시 에러 해결 추후 변경 필요
            if (_sceneData.storyType == StoryScene.StoryType.VisualNovel && i >= data.Length - 2)
            {
                break;
            }
            
            StoryScene.Sentence dialogue = new StoryScene.Sentence();

            if (row[_CSV_SPEAKER_INDEX] != "")
            {
                // 만약 speaker 딕셔너리에 현재 화자의 값이 없으면 추가. 
                if (!_speakerDictionary.ContainsKey(row[_CSV_SPEAKER_INDEX]))
                {
                    _speakerDictionary.Add(row[_CSV_SPEAKER_INDEX],
                        Resources.Load<Speaker>("Speaker/" + row[_CSV_SPEAKER_INDEX].Trim()));
                }
                _lastSpeaker = _speakerDictionary[row[_CSV_SPEAKER_INDEX].Trim()];
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
            StoryScene.Sentence.Action action = GetActionParse(row);
            
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

    private StoryScene.Sentence.Action GetActionParse(string[] row)
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
}
