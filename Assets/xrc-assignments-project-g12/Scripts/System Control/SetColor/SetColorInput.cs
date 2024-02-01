using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G12
{
    public class SetColorInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference m_RightPrimaryButtonInputActionRef;
        [SerializeField] private SetColor m_SetColor;
        
        private void OnEnable()
        {
            m_RightPrimaryButtonInputActionRef.action.performed += ToggleColorPickerVisibility;
        }

        private void OnDisable()
        {
            m_RightPrimaryButtonInputActionRef.action.performed -= ToggleColorPickerVisibility;
        }
        
        private void ToggleColorPickerVisibility(InputAction.CallbackContext context)
        {
            m_SetColor.ToggleColorPickerVisibility(); 
        }
    }
}