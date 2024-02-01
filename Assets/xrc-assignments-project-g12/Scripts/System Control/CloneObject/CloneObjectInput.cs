using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G12
{
    public class CloneObjectInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference m_TriggerInputActionRef;
        [SerializeField] private CloneObject m_CloneObject;
        [SerializeField] private TargetProvider m_TargetProvider;
        [SerializeField] private RayInteractorEventDistributor m_RayInteractorEventDistributor;
        
        private void OnEnable()
        {
            m_TriggerInputActionRef.action.Enable();
            m_TriggerInputActionRef.action.performed += OnTriggerPressed;
        }

        private void OnDisable()
        {
            m_TriggerInputActionRef.action.Enable();
            m_TriggerInputActionRef.action.performed += OnTriggerPressed;
        }
        
        private void OnTriggerPressed(InputAction.CallbackContext ctx)
        {
            if (m_RayInteractorEventDistributor.State != RayInteractorState.Idle ||
                    m_TargetProvider.CurrentTarget == null)
            {
                return;
            }

            m_CloneObject.Clone(m_TargetProvider.CurrentTarget);
        }
    }
}
