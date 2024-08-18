using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 애니메이션 상수를 관리하는 클래스
/// </summary>
public class AnimationConstants
{
	[Header("Player Animation IDs")]
    public static readonly int AnimIDSpeed 		    = Animator.StringToHash("Speed");
    public static readonly int AnimIDGrounded 		= Animator.StringToHash("Grounded");
    public static readonly int AnimIDJump 			= Animator.StringToHash("Jump");
    public static readonly int AnimIDFreeFall 		= Animator.StringToHash("FreeFall");
    public static readonly int AnimIDMotionSpeed 	= Animator.StringToHash("MotionSpeed");
}
