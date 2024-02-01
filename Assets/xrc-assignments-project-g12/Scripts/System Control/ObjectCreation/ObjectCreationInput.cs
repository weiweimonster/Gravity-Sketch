using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace XRC.Assignments.Project.G12
{
    public class ObjectCreationInput : MonoBehaviour
    {
        [Tooltip("Object Creator")] [SerializeField]
        private ObjectCreation m_ObjectCreation;
        [SerializeField] private InputActionReference m_TriggerInputActionRef;
        [SerializeField] private RayInteractorEventDistributor m_RayInteractorEventDistributor;
        [SerializeField] private SphereSelect m_SphereSelect;
        [SerializeField] private EditModeManager m_EditModeManager;
        
        
        private bool isTriggerPressed = false;

        // Update is called once per frame
        void Update()
        {
            if (m_EditModeManager.CurrentState != EditModeStates.Idle)
            {
                return;
            }
            // Check if the ray interactor is in Idle state, if yes, enable object creation
            if ((m_RayInteractorEventDistributor.State == RayInteractorState.Idle) & (!m_SphereSelect.IsGrabbing()))
            {
                // Check if the grip button is held down
                isTriggerPressed = m_TriggerInputActionRef.action.phase == InputActionPhase.Performed;
                if (isTriggerPressed)
                {
                    m_ObjectCreation.UpdateDistanceBetweenControllers();
                }
                else if (!isTriggerPressed && m_ObjectCreation.Distance > 0)
                {
                    m_ObjectCreation.CreateObject();
                }
            }
        }
    }
}