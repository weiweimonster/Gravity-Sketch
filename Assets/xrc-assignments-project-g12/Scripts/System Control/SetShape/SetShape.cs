using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G12
{
    public class SetShape : MonoBehaviour
    {
        [SerializeField] private InputActionReference m_LeftPrimaryButtonInputActionRef;
        [SerializeField] private List<GameObject> m_ShapePrefabs;
        
        private bool isShapesVisible = false;

        private void OnEnable()
        {
            m_LeftPrimaryButtonInputActionRef.action.performed += ToggleShapesVisibility;
        }

        private void OnDisable()
        {
            m_LeftPrimaryButtonInputActionRef.action.performed -= ToggleShapesVisibility;
        }

        private void ToggleShapesVisibility(InputAction.CallbackContext context)
        {
            isShapesVisible = !isShapesVisible;
            Debug.Log("Toggle Shapes Visibility: " + isShapesVisible);
            foreach (var shape in m_ShapePrefabs)
            {
                if (shape != null)
                {
                    shape.SetActive(isShapesVisible);
                }
            }
        }
        
        // =================== Public Methods ===================
        public List<GameObject> ShapePrefabs
        {
            get => m_ShapePrefabs;
        }
    }
}
