using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 싱글톤 패턴을 구현하기 위한 제네릭 클랙스
/// </summary>
[DisallowMultipleComponent]
public class BasicSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T SingletonObject;	// 싱글톤 객체를 저장할 변수

	/// <summary>
	/// 싱글톤 객체를 반환하는 프로퍼티
	/// </summary>
	public static T Instance {
		get{
			if(SingletonObject == null){
				if((SingletonObject = FindObjectOfType<T>()) == null){
					throw new MissingReferenceException($"{typeof(T).Name} 타입을 가진 싱글톤 객체를 찾을 수 없습니다.");
				}
			}

			return SingletonObject;
		}
	}

	/// <summary>
	/// 싱글톤 객체가 존재하는지 확인하는 함수
	/// </summary>
	public static bool HasSingletonObject{
		get{
			if(SingletonObject == null){
				if((SingletonObject = FindObjectOfType<T>()) == null){
					return false;
				}
			}

			return true;
		}
	}

	protected void Reset(){
		#if UNITY_EDITOR
		if(FindObjectsOfType<T>().Length > 1){
			EditorUtility.DisplayDialog("Singleton Object Error", "Singleton 객체는 하나만 존재해야 합니다.", "확인");
			DestroyImmediate(this);
		}
		#endif
	}
}
