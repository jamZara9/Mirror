using UnityEngine;

namespace Utils{

    public class MouseUtil
    {
        /// <summary>
        /// 마우스 커서를 숨기고 잠그는 함수
        /// </summary>
        public static void LockAndHideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// 마우스 커서를 풀고 보이게 하는 함수
        /// </summary>
        public static void UnlockAndShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public class MathUtil
    {
        /// <summary>
        /// 각도 제한
        /// </summary>
        /// <param name="lfAngle"></param>
        /// <param name="lfMin"></param>
        /// <param name="lfMax"></param>
        /// <returns></returns>
        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }

    public class ComponentUtil
    {
        /// <summary>
        /// 자식 오브젝트를 찾아 반환하는 함수
        /// </summary>
        /// <param name="parent">부모 오브젝트</param>
        /// <param name="name">찾을 자식 오브젝트 이름</param>
        /// <returns>찾은 자식 오브젝트</returns>
        public static Transform FindChildObject(GameObject parent, string name)
        {
            Transform[] trs = parent.GetComponentsInChildren<Transform>(true);

            foreach (Transform tr in trs)
            {
                if (tr.name == name)
                {
                    return tr;
                }
            }

            return null;
        }

        /// <summary>
        /// 특정 컴포넌트를 찾아 반환, 없을 경우 새로 추가
        /// </summary>
        /// <typeparam name="T">가져오려는 컴포넌트 타입</typeparam>
        /// <param name="obj">컴포넌트를 찾을 기준 오브젝트</param>
        /// <returns>컴포넌트가 존재하면 해당 컴포넌트, 없으면 새로 추가된 컴포넌트를 반환</returns>
        public static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent<T>(out var component))
            {
                component = obj.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// 특정 컴포넌트를 활성화/비활성화 하는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트만 제어할 수 있도록</typeparam>
        /// <param name="obj">활성화/비활성화를 제어할 오브젝트</param>
        /// <param name="isActive">활성화 여부</param>
        public static void SetComponentActive<T>(GameObject obj, bool isActive) where T : Component
        {
            if (obj.TryGetComponent<T>(out var component))
            {
                component.gameObject.SetActive(isActive);
            }
        }
    }    
}
