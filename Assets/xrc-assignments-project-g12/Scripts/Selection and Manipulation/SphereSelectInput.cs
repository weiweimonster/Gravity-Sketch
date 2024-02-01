using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class SphereSelectInput : MonoBehaviour
    {
        [SerializeField] private float m_inputSensitivity;
        [SerializeField] private InputActionReference m_JoystickReference;
        [SerializeField] private SphereSelect m_SphereSelect;
        [SerializeField] private EditModeManager m_EditModeManager;
        
        
        private void OnEnable()
        {
            m_JoystickReference.action.Enable();
            m_JoystickReference.action.performed += OnJoystickPerformed;
        }

        private void OnDisable()
        {
            m_JoystickReference.action.Disable();
            m_JoystickReference.action.performed -= OnJoystickPerformed;
        }
    
        private void OnJoystickPerformed(InputAction.CallbackContext ctx)
        {
            if (m_SphereSelect.IsGrabbing())
            {
                return;
            }
            Vector2 inputVector = ctx.ReadValue<Vector2>();
            Debug.Log("Joystick input: " + inputVector);
            float localScale = gameObject.transform.localScale.y;
            localScale += inputVector.y * m_inputSensitivity * Time.deltaTime;
            m_SphereSelect.SetRadius(localScale);
        }
        
        // Update is called once per frame
        void Update()
        {
            if (m_EditModeManager.CurrentState == EditModeStates.ScalePerAxis)
            {
                return;
            }
            if (m_SphereSelect.IsGrabbing())
            {
                m_SphereSelect.Grab();
            }
        }
        
    }
}
