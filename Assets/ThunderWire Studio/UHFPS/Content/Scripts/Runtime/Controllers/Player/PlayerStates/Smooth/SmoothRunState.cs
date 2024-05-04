using UnityEngine;
using UHFPS.Input;
using UHFPS.Scriptable;

namespace UHFPS.Runtime.States
{
    public class SmoothRunState : SmoothStateAsset
    {
        public override FSMPlayerState InitState(PlayerStateMachine machine, PlayerStatesGroup group)
        {
            return new RunningPlayerState(machine, group);
        }

        public override string StateKey => PlayerStateMachine.RUN_STATE;

        public override string Name => "Smooth/Run";

        public class RunningPlayerState : SmoothPlayerState
        {
            public RunningPlayerState(PlayerStateMachine machine, PlayerStatesGroup group) : base(machine, group)
            {
            }

            public override void OnStateEnter()
            {
                movementSpeed = machine.PlayerBasicSettings.RunSpeed;
                controllerState = machine.StandingState;
                InputManager.ResetToggledButton("Crouch", Controls.CROUCH);
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To(PlayerStateMachine.IDLE_STATE, () =>
                    {
                        return InputMagnitude <= 0;
                    }),
                    Transition.To(PlayerStateMachine.WALK_STATE, () =>
                    {
                        if(MovementInput.y < 0 || (MovementInput.y == 0 && MovementInput.x != 0))
                            return true;

                        if(InputMagnitude > 0)
                        {
                            if (machine.PlayerFeatures.RunToggle)
                            {
                                bool runToggle = !InputManager.ReadButtonToggle("Run", Controls.SPRINT);
                                return runToggle || (StaminaEnabled && machine.Stamina.Value <= 0f);
                            }

                            bool runUnPressed = !InputManager.ReadButton(Controls.SPRINT);
                            return runUnPressed || (StaminaEnabled && machine.Stamina.Value <= 0f);
                        }

                        return false;
                    }),
                    Transition.To(PlayerStateMachine.CROUCH_STATE, () =>
                    {
                        if (machine.PlayerFeatures.CrouchToggle)
                        {
                            return InputManager.ReadButtonToggle("Crouch", Controls.CROUCH);
                        }

                        return InputManager.ReadButton(Controls.CROUCH);
                    }),
                    Transition.To(PlayerStateMachine.JUMP_STATE, () =>
                    {
                        bool jumpPressed = InputManager.ReadButtonOnce("Jump", Controls.JUMP);
                        return jumpPressed && (!StaminaEnabled || machine.Stamina.Value > 0f);
                    }),
                    Transition.To(PlayerStateMachine.SLIDING_STATE, () =>
                    {
                        if(SlopeCast(out _, out float angle))
                            return angle > machine.PlayerSliding.SlopeLimit;

                        return false;
                    }),
                    Transition.To(PlayerStateMachine.DEATH_STATE, () => IsDead)
                };
            }
        }
    }
}