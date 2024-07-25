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

		public bool[] quickSlots = new bool[5];

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

		public void OnQuickSlot(InputValue value)
		{
			QuickSlotInput(value);
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

		private void QuickSlotInput(InputValue value)
		{
			int slotIndex = value.Get<int>();

			for (int i = 0; i < quickSlots.Length; i++){
				quickSlots[i] = false;
			}

			if (slotIndex >= 0 && slotIndex < quickSlots.Length){
				quickSlots[slotIndex - 1] = value.isPressed;
			}
		}
	}
	
}