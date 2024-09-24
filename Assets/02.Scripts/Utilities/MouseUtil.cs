using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
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
}
