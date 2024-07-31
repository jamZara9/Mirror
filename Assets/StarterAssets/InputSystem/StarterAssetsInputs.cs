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

		public int number = 0;

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
		public void OnInventory(InputValue value)
		{
			ShowInventory(value.isPressed);
		}

		public void OnNumber1(InputValue value)
		{
			ClickNumber(value.isPressed, 1);
		}
		public void OnNumber2(InputValue value)
		{
			ClickNumber(value.isPressed, 2);
		}
		public void OnNumber3(InputValue value)
		{
			ClickNumber(value.isPressed, 3);
		}
		public void OnNumber4(InputValue value)
		{
			ClickNumber(value.isPressed, 4);
		}
		public void OnNumber5(InputValue value)
		{
			ClickNumber(value.isPressed, 5);
		}
		public void OnNumber6(InputValue value)
		{
			ClickNumber(value.isPressed, 6);
		}
		public void OnNumber7(InputValue value)
		{
			ClickNumber(value.isPressed, 7);
		}	
		public void OnNumber8(InputValue value)
		{
			ClickNumber(value.isPressed, 8);
		}
		public void OnNumber9(InputValue value)
		{
			ClickNumber(value.isPressed, 9);
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
		
		private void ShowInventory(bool newInventoryState){
			inventory = newInventoryState;
		}

		private void ClickNumber(bool newNumberState, int number){
			if(newNumberState){
				this.number = number;
			}
		}
	}
	
}