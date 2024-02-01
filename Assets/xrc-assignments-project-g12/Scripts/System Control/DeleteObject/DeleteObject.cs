using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using XRC.Assignments.Project.G12;

namespace XRC.Assignments.Project.G12
{
    public class DeleteObject : MonoBehaviour
    {
        [SerializeField] private InputActionReference m_RightSecondaryButtonInputActionRef;
        [SerializeField] private SphereSelect m_SphereSelect;
        [SerializeField] private EditModeManager m_EditModeManager;
        


        private void OnEnable()
        {
            m_RightSecondaryButtonInputActionRef.action.performed += DeleteObjectAction;
        }

        private void OnDisable()
        {
            m_RightSecondaryButtonInputActionRef.action.performed -= DeleteObjectAction;
        }

        private void DeleteObjectAction(InputAction.CallbackContext context)
        {
            if (m_EditModeManager.CurrentState == EditModeStates.ScalePerAxis)
            {
                return;
            }
            if (m_SphereSelect.IsGrabbing())
            {
                m_SphereSelect.DeleteGrabbedInteractables();
            }
        }
    }
}
