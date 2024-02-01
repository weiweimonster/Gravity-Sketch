using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class RayInteractorEventDistributor : MonoBehaviour
    {
        [SerializeField] private InputActionReference m_TriggerInputActionRef;
        [SerializeField] private SetShape m_SetShape;
        [SerializeField] private SetColor m_SetColor;
        [SerializeField] private HelpAndDocumentation m_HelpAndDocumentation;
        
        private XRBaseInteractor rayInteractor;
        private RayInteractorState m_RayInteractorState = RayInteractorState.Idle;
        
        // for selecting shape
        private int? m_CurrentPrefabIndex;
        
        
        public event Action<GameObject> OnShapeChanged;

        void Start()
        {
            rayInteractor = GetComponent<XRBaseInteractor>();
            rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
            rayInteractor.hoverExited.AddListener(HandleHoverExited);
        }
        
        private void OnDestroy()
        {
            rayInteractor.hoverEntered.RemoveListener(HandleHoverEntered);
            rayInteractor.hoverExited.RemoveListener(HandleHoverExited);
        }

        private void OnEnable()
        {
            m_TriggerInputActionRef.action.performed += HandleTriggerPressed;

            m_SetColor.OnColorPickerActivated += HandleColorPickerActivated;
            m_SetColor.OnColorPickerDeactivated += HandleColorPickerDeactivated;
        }

        private void OnDisable()
        {
            m_TriggerInputActionRef.action.performed -= HandleTriggerPressed;
            
            m_SetColor.OnColorPickerActivated -= HandleColorPickerActivated;
            m_SetColor.OnColorPickerDeactivated -= HandleColorPickerDeactivated; 
        }
        
        // =================== Private Methods ===================
        
        private void HandleHoverEntered(HoverEnterEventArgs arg)
        {
            // check if the object is in m_ObjectCreationPrefabs
            // if yes, invoke "ChangeShape(shapeType)" event for specific script to subscribe IF click the trigger button on right hand
            // else, no-op
            // you can extend this method to handle other events whichever triggered by ray interactor
            Debug.Log("Hover Entered: " + arg.interactableObject);
            if (m_SetShape.ShapePrefabs.Contains(arg.interactableObject.transform.gameObject))
            {
                Debug.Log("Hover Entered: " + arg.interactableObject);
                // get the index of the object in m_ObjectCreationPrefabs
                int index = m_SetShape.ShapePrefabs.IndexOf(arg.interactableObject.transform.gameObject);
                m_CurrentPrefabIndex = index;
                // change state to selecting shape
                m_RayInteractorState = RayInteractorState.SelectingShape;
            } else if (m_HelpAndDocumentation.HelpButton == arg.interactableObject.transform.gameObject)
            {
                Debug.Log("Hover Entered: " + arg.interactableObject);
                m_RayInteractorState = RayInteractorState.SelectingHelp;
            }
        }

        private void HandleHoverExited(HoverExitEventArgs arg)
        {
            if (m_SetShape.ShapePrefabs.Contains(arg.interactableObject.transform.gameObject))
            {
                // get the index of the object in m_ObjectCreationPrefabs
                int index = m_SetShape.ShapePrefabs.IndexOf(arg.interactableObject.transform.gameObject);
                if (index == m_CurrentPrefabIndex)
                {
                    m_CurrentPrefabIndex = null;
                    // change state to idle
                    m_RayInteractorState = RayInteractorState.Idle;
                }
            }  else if (m_HelpAndDocumentation.HelpButton == arg.interactableObject.transform.gameObject)
            {
                Debug.Log("Hover Exited: " + arg.interactableObject);
                m_RayInteractorState = RayInteractorState.Idle;
            }
        }

        private void HandleTriggerPressed(InputAction.CallbackContext obj)
        {
            // TODO: invoke events for change shape
            if (m_CurrentPrefabIndex != null)
            {
                Debug.Log("Trigger Pressed");
                Debug.Log("Change Shape: " + m_SetShape.ShapePrefabs[(int) m_CurrentPrefabIndex]);
                OnShapeChanged?.Invoke(m_SetShape.ShapePrefabs[(int) m_CurrentPrefabIndex]);
            }
        }
        
        private void HandleColorPickerActivated() {
           m_RayInteractorState = RayInteractorState.SelectingColor; 
        } 
        
        private void HandleColorPickerDeactivated() {
            m_RayInteractorState = RayInteractorState.Idle;
        }
        
        // =================== Public Methods ===================
        public RayInteractorState State => m_RayInteractorState;
    }
}
