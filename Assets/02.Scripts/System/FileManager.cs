using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    /// <summary>
    /// 해당 경로의 파일을 읽어서 string으로 반환
    /// </summary>
    /// <param name="path">경로</param>
    /// <returns></returns>
    public static string LoadJsonFile(string path){
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        if (jsonTextAsset == null){
            Debug.LogError($"파일이 존재하지 않습니다: {jsonTextAsset}");
            return null;
        }
        return jsonTextAsset.text;
    }
}
