using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace xrc_assignments_project_g12.Scripts.Locomotion
{
    public class MyGrabMove: MonoBehaviour
    {   
        
        [SerializeField]
        private List<Transform> environment;
        [Tooltip("Left Controller")] [SerializeField]
        private ActionBasedController leftController;

        public ActionBasedController lController
        {
            get => leftController;
            set => leftController = value;
        }
        [Tooltip("Right Controller")] [SerializeField]
        private ActionBasedController rightController;
        public ActionBasedController rController
        {
            get => rightController;
            set => rightController = value;
        }
        [Tooltip("XR Rig")] [SerializeField]
        private Transform xrRig;
        
        public Transform rig
        {
            get => xrRig;
            set => xrRig = value;
        }
        
        [SerializeField]
        [Tooltip("The ratio of actual movement distance to controller movement distance.")]
        float m_MoveFactor = 1f;
        
        [SerializeField]
        [Tooltip("Controls whether to enable uniform scaling of the user.")]
        bool m_EnableScaling;
        
        [SerializeField]
        [Tooltip("The minimum user scale allowed.")]
        float m_MinimumScale = 0.1f;
        
        [SerializeField]
        [Tooltip("The maximum user scale allowed.")]
        float m_MaximumScale = 100f;
        
        bool m_IsMoving = false;

        private Quaternion m_InitialOriginRotation;
        private Quaternion m_InitialEnvironmentRotation;
        Vector3 m_PreviousMidpointBetweenControllers;

        float m_InitialOriginScale;
        float m_InitialDistanceBetweenHands;
        
        
        private Vector3 m_InitialLefttoRight;
        
        
        [SerializeField]
        [Tooltip("The Input System Action that will be used to perform grab movement while held. Must be a Button Control.")]
        InputActionProperty m_LeftGrabMoveAction = new InputActionProperty(new InputAction("Grab Move", type: InputActionType.Button));
        
        public InputActionProperty leftGrabMoveAction
        {
            get => m_LeftGrabMoveAction;
            set => SetInputActionProperty(ref m_LeftGrabMoveAction, value);
        }
        
        [SerializeField]
        [Tooltip("The Input System Action that will be used to perform grab movement while held. Must be a Button Control.")]
        InputActionProperty m_RightGrabMoveAction = new InputActionProperty(new InputAction("Grab Move", type: InputActionType.Button));
        
        public InputActionProperty rightGrabMoveAction
        {
            get => m_RightGrabMoveAction;
            set => SetInputActionProperty(ref m_RightGrabMoveAction, value);
        }
        protected void OnEnable()
        {
            m_LeftGrabMoveAction.EnableDirectAction();
            m_RightGrabMoveAction.EnableDirectAction();
        }

        protected void OnDisable()
        {
            m_LeftGrabMoveAction.DisableDirectAction();
            m_RightGrabMoveAction.DisableDirectAction();
        }

        private bool IsGrabbing()
        {
            return m_RightGrabMoveAction.action.IsPressed() && m_LeftGrabMoveAction.action.IsPressed();
        }

        private void OnBeginLocomotion()
        {
            if (!IsGrabbing()) return;
            var originTransform = xrRig;
            var leftHandLocalPosition = leftController.transform.localPosition;
            var rightHandLocalPosition = rightController.transform.localPosition;
            
            // handle rotation
            var currentHand = rightHandLocalPosition - leftHandLocalPosition;
            Quaternion rotation = Quaternion.FromToRotation(currentHand, m_InitialLefttoRight);
            originTransform.rotation = m_InitialOriginRotation * rotation;
            environment[0].transform.rotation = m_InitialEnvironmentRotation * rotation;
            
            
            if (m_EnableScaling)
            {
                var offset = CalculateOffset();
                var distanceBetweenHands = Vector3.Distance(leftHandLocalPosition, rightHandLocalPosition);
                var targetScale = distanceBetweenHands != 0f
                    ? m_InitialOriginScale * (m_InitialDistanceBetweenHands / distanceBetweenHands)
                    : originTransform.localScale.x;
            
                targetScale = Mathf.Clamp(targetScale, m_MinimumScale, m_MaximumScale);
                var initialScale = originTransform.localScale.x;
                originTransform.localScale = Vector3.one * targetScale;
                environment[0].localScale = Vector3.one * targetScale; 
                // var pos = originTransform.position;
                // originTransform.position = new Vector3(pos.x, pos.y - (targetScale - initialScale) * 1.0f, pos.z);
                // environment[0].position = new Vector3(pos.x, pos.y - (targetScale - initialScale) * 1.0f, pos.z);
                RemoveOffset(offset);
            }
        }
        
        private Vector3 ComputeDesiredMove()
        {
            var wasMoving = m_IsMoving;
            m_IsMoving = IsGrabbing();
            if (!m_IsMoving) return Vector3.zero;
            var originTransform = xrRig;
            var leftHandLocalPosition = leftController.transform.localPosition;
            var rightHandLocalPosition = rightController.transform.localPosition;
            var midpointLocalPosition = (leftHandLocalPosition + rightHandLocalPosition) * 0.5f;
            
            if (!wasMoving && m_IsMoving)
            {
                m_InitialLefttoRight = rightHandLocalPosition - leftHandLocalPosition;
                m_InitialOriginRotation = originTransform.rotation;
                m_InitialEnvironmentRotation = environment[0].rotation;
                
                m_InitialOriginScale = originTransform.localScale.x;
                m_InitialDistanceBetweenHands = Vector3.Distance(leftHandLocalPosition, rightHandLocalPosition);

                // Do not move the first frame of grab
                m_PreviousMidpointBetweenControllers = midpointLocalPosition;
                return Vector3.zero;
            }

            var move = originTransform.TransformVector(m_PreviousMidpointBetweenControllers - midpointLocalPosition) *
                       m_MoveFactor;
            m_PreviousMidpointBetweenControllers = midpointLocalPosition;
            return move;
        }

        private void OnMoveCharacter(Vector3 move)
        {
            xrRig.position += move / xrRig.localScale.x;
            environment[0].position += move / xrRig.localScale.x;
        }
        
        private void Update()
        {
            var move = ComputeDesiredMove();
            OnMoveCharacter(move);
            OnBeginLocomotion();
            
            
        }

        
        void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }

        private Vector3 CalculateOffset()
        {
            var referenceSpere = environment[1];
            return referenceSpere.position - Camera.main.transform.position;
        }


        private void RemoveOffset(Vector3 prevOffset)
        {
            var pos = rig.position;
            var envPos = environment[0].position;
            var referenceSpere = environment[1];
            var currOffset = referenceSpere.position.y - Camera.main.transform.position.y;
            var diff = currOffset - prevOffset.y;
            pos.y += diff;
            envPos.y += diff;
            rig.position = pos;
            environment[0].position = envPos;
        }

        
    }
}