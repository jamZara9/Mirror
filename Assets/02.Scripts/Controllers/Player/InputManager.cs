using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public InputActionAsset inputAction;                            // InputActionAsset
    private Dictionary<string, InputActionMap> actionMaps;          // ActionMap을 저장할 딕셔너리
    [SerializeField] private InputActionMap currentActionMap;       // 현재 활성화된 ActionMap

    private void Awake()
    {
        if (!inputAction) throw new NullReferenceException("InputActionAsset이 할당되지 않았습니다.");

        inputAction.Enable();              // 최초 모든 액션 맵을 활성화

        actionMaps = new Dictionary<string, InputActionMap>();  // 딕셔너리 초기화
        foreach (var map in inputAction.actionMaps)             // 모든 ActionMap을 순회하며
        {
            actionMaps.Add(map.name, map);                      // 딕셔너리에 추가
        }

        SwitchActionMap("Player");
    }

    /// <summary>
    /// 해당 이름의 ActionMap을 활성화
    /// </summary>
    /// <param name="mapName">활성화하고자 하는 ActionMap</param>
    public void SwitchActionMap(string mapName)
    {
        // var map = inputAction.FindActionMap(mapName);
        // if (map == null)
        // {
        //     Debug.LogError($"{mapName}의 이름을 가진 ActionMap을 찾을 수 없습니다.");
        //     return;
        // }

        // if (currentActionMap != null)
        // {
        //     Debug.Log("현재 ActionMap 비활성화");
        //     currentActionMap.Disable();
        // }

        // map.Enable();                           // 새로운 ActionMap 활성화
        // currentActionMap = map;                 // 현재 ActionMap 변경

        var map = actionMaps[mapName];
        if (map == null)
        {
            Debug.LogError($"{mapName}의 이름을 가진 ActionMap을 찾을 수 없습니다.");
            return;
        }

        if (currentActionMap != null)
        {
            Debug.Log("현재 ActionMap 비활성화");
            currentActionMap.Disable();
        }

        map.Enable();                           // 새로운 ActionMap 활성화
        currentActionMap = map;                 // 현재 ActionMap 변경
    }

    /// <summary>
    /// 현재 활성화된 ActionMap의 이름을 반환
    /// </summary>
    /// <returns></returns>
    public string GetCurrentActionMapName() => currentActionMap?.name;
}
