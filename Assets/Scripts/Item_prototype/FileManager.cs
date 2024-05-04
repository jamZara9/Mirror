using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FileManager
{
    /// <summary>
    /// 해당 경로의 파일을 읽어서 string으로 반환
    /// </summary>
    /// <param name="path">경로</param>
    /// <returns></returns>
    public static string LoadJsonFile(string path){
        return File.ReadAllText(path);
    }
}
