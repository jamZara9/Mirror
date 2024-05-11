using System;
using UnityEngine;
using Cinemachine;

namespace UHFPS.Runtime
{
    public abstract class PlayerComponent : MonoBehaviour
    {
        protected bool isEnabled = true;

        public virtual void SetEnabled(bool enabled)
        {
            isEnabled = enabled;
        }

        [NonSerialized]
        private PlayerManager playerManager;
        public PlayerManager PlayerManager
        {
            get
            {
                if (playerManager == null)
                {
                    Transform currentTransform = transform;
                    while (currentTransform != null)
                    {
                        if (currentTransform.TryGetComponent(out playerManager)) 
                            break;

                        currentTransform = currentTransform.parent;
                    }
                }

                return playerManager;
            }
        }

        [NonSerialized]
        private CharacterController playerCollider;
        public CharacterController PlayerCollider
        {
            get
            {
                if (playerCollider == null)
                    playerCollider = PlayerManager.GetComponent<CharacterController>();

                return playerCollider;
            }
        }

        [NonSerialized]
        private PlayerStateMachine playerStateMachine;
        public PlayerStateMachine PlayerStateMachine
        {
            get
            {
                if (playerStateMachine == null)
                    playerStateMachine = PlayerManager.GetComponent<PlayerStateMachine>();

                return playerStateMachine;
            }
        }

        [NonSerialized]
        private LookController lookController;
        public LookController LookController
        {
            get
            {
                if (lookController == null)
                    lookController = PlayerManager.GetComponentInChildren<LookController>();

                return lookController;
            }
        }

        [NonSerialized]
        private ExamineController examineController;
        public ExamineController ExamineController
        {
            get
            {
                if (examineController == null)
                    examineController = PlayerManager.GetComponentInChildren<ExamineController>();

                return examineController;
            }
        }

        public Camera MainCamera => PlayerManager.MainCamera;
        public CinemachineVirtualCamera VirtualCamera => PlayerManager.MainVirtualCamera;
    }
}