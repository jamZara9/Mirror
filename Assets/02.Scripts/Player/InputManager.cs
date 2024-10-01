using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;

public class InputManager : IManager
{
    private InputActionAsset inputAction;                       // InputActionAsset
    private Dictionary<string, InputActionMap> actionMaps;      // ActionMap을 저장할 딕셔너리
    private InputActionMap currentActionMap;                    // 현재 활성화된 ActionMap

    private Dictionary<string, IInputActionStrategy> _inputStrategies; // 바인딩된 InputAction을 저장할 딕셔너리
    
    // IManager 인터페이스 구현
    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.StartScene){

            inputAction = Resources.Load<InputActionAsset>("Input/PlayerInputActions");

            if (!inputAction)
            {
                throw new NullReferenceException("InputActionAsset이 할당되지 않았습니다.");
            }

            inputAction.Enable(); // 모든 액션 맵을 활성화

            // 딕셔너리 초기화
            actionMaps = new Dictionary<string, InputActionMap>();
            _inputStrategies = new Dictionary<string, IInputActionStrategy>();

            // 모든 ActionMap을 딕셔너리에 추가
            foreach (var map in inputAction.actionMaps)
            {
                actionMaps.Add(map.name, map);
            }

            // InputACtion 초기화하고 저장
            _inputStrategies.Add("Player", new PlayerInputAction());
            _inputStrategies.Add("Dialog", new DialogueInputAction());
            _inputStrategies.Add("AnyKey", new AnyKeyInputAction()); 

            // 초기 상태로 AnyKey ActionMap 활성화
            SwitchActionMap("Player");
        }
        
    }

    /// <summary>
    /// 해당 이름의 ActionMap을 활성화
    /// </summary>
    /// <param name="mapName">활성화하고자 하는 ActionMap</param>
    public void SwitchActionMap(string mapName)
    {
        if (!_inputStrategies.ContainsKey(mapName))
        {
            Debug.LogError($"{mapName}의 InputActionStrategy가 존재하지 않습니다.");
            return;
        }

        // 기존 액션 맵이 존재하면 비활성화
        currentActionMap?.Disable();

        // 해당 이름의 액션 맵을 활성화
        var map = actionMaps[mapName];  
        map.Enable();                   
        currentActionMap = map;         

        var strategy = _inputStrategies[mapName];
        strategy.BindInputActions(map);
    }

    /// <summary>
    /// 현재 활성화된 ActionMap의 이름을 반환
    /// </summary>
    public string GetCurrentActionMapName() => currentActionMap?.name;

    /// <summary>
    /// 해당 이름을 가진 InputActionStrategy를 반환
    /// </summary>
    /// <param name="mapName"></param>
    /// <returns></returns>
    public IInputActionStrategy GetInputActionStrategy(string mapName)
    {
        if (!_inputStrategies.ContainsKey(mapName))
        {
            Debug.LogError($"{mapName}의 InputActionStrategy가 존재하지 않습니다.");
            return null;
        }

        return _inputStrategies[mapName];
    }

    /// <summary>
    /// 해당 이름의 ActionMap에 있는 모든 Action을 바인딩
    /// </summary>
    /// <param name="mapName"></param>
    /// <param name="target"></param>
    public void BindAllActions(string mapName, object target)
    {
        var map = actionMaps[mapName];
        if (map != null)
        {
            foreach (var action in map.actions)
            {
                var method = target.GetType().GetMethod("On" + action.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    var del = Delegate.CreateDelegate(typeof(Action<InputAction.CallbackContext>), target, method);
                    action.performed += (Action<InputAction.CallbackContext>)del;
                    action.canceled += (Action<InputAction.CallbackContext>)del;
                }
                else
                {
                    Debug.LogWarning($"Action과 매칭되는 Method를 찾을 수 없습니다. : {action.name}");
                }
            }
        }
        else
        {
            Debug.LogError("해당 이름을 가진 ActionMap을 찾을 수 없습니다.");
            Debug.LogError($"{mapName}의 InputActionStrategy가 존재하지 않습니다.");
            return;
        }
    }

}
