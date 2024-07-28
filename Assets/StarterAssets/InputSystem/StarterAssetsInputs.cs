using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		// test code
		public bool useItem;
		public bool pickupItem;
		public bool transferItem;
		public bool inventory;

		public int quickSlots;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnUseItem(InputValue value)
		{
			UseItemInput(value.isPressed);
		}

		public void OnPickupItem(InputValue value)
		{
			PickupItemInput(value.isPressed);
		}

		public void OnTransferItem(InputValue value)
		{
			TransferItemInput(value.isPressed);
		}

		public void OnQuickSlots(InputValue value)
		{
			QuickSlotInput(value);
		}

		public void OnInventory(InputValue value)
		{
			ShowInventory(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		private void UseItemInput(bool newUseItemState)
		{
			useItem = newUseItemState;
		}

		private void PickupItemInput(bool newPickupItemState)
		{
			pickupItem = newPickupItemState;
		}

		private void TransferItemInput(bool newTransferItemState)
		{
			transferItem = newTransferItemState;
		}

		/// <summary>
		/// 퀵슬롯 입력 처리
		/// </summary>
		/// <param name="value"></param>
		/// @TODO : 입력받은 퀵슬롯 번호를 저장해야 함 현재는 입력받았는지 즉 0(KeyDown) or 1(KeyUp)만 확인된 상태
		/// @TODO : 하나로 바인딩 처리할 때 입력받은 번호를 가져올 수 있도록 수정 필요
		private void QuickSlotInput(InputValue value)
		{
			quickSlots = (int) value.Get<float>();
			Debug.Log($"QuickSlot: {quickSlots}");
		}

		private void ShowInventory(bool newInventoryState){
			inventory = newInventoryState;
		}
	}
	
}