using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class ScaleAxis : MonoBehaviour
    {
        [SerializeField] private TargetProvider m_TargetProvider;
        [SerializeField] private SphereSelect m_SphereSelect;
        [SerializeField] private InputActionReference m_LeftSecondaryButtonRef;
        [SerializeField] private EditModeManager m_EditModeManager;
        [Tooltip("Right Controller")] [SerializeField]
        private ActionBasedController rightController;
        
        
        private XRBaseInteractable m_CurrentEditInteractable;
        private Axis m_CurrentScaleAxis = Axis.None;
        private Vector3? m_prevRightHandPosition = null;
        
        // Handle prefabs
        [SerializeField] private GameObject xAxisHandlePrefab;
        [SerializeField] private GameObject yAxisHandlePrefab;
        [SerializeField] private GameObject zAxisHandlePrefab;

        private GameObject xAxisHandle;
        private GameObject yAxisHandle;
        private GameObject zAxisHandle;
        
        private void OnEnable()
        {
            m_LeftSecondaryButtonRef.action.performed += OnLeftSecondaryButtonPressed;
            m_SphereSelect.onScaleHandleGrab += HandleScaleHandleGrab;
            m_SphereSelect.onScaleHandleRelease += HandleScaleHandleRelease;
        }
        
        private void OnDisable()
        {
            m_LeftSecondaryButtonRef.action.performed -= OnLeftSecondaryButtonPressed;
            m_SphereSelect.onScaleHandleGrab -= HandleScaleHandleGrab;
            m_SphereSelect.onScaleHandleRelease -= HandleScaleHandleRelease;
        }

        private void OnLeftSecondaryButtonPressed(InputAction.CallbackContext obj)
        {
            // if not in Scale mode, enter it; else, exit it
            if (m_EditModeManager.CurrentState != EditModeStates.ScalePerAxis)
            {
                if (m_TargetProvider.CurrentTarget == null) return;
                if (!m_SphereSelect.IsGrabbing()) return;
                
                m_EditModeManager.ChangeState(EditModeStates.ScalePerAxis);
                m_CurrentEditInteractable = m_TargetProvider.CurrentTarget;
                
                Debug.Log("!!!");
                // Disable grabbing for the current interactable
                XRGrabInteractable grabInteractable = m_CurrentEditInteractable.GetComponent<XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = false;
                }
                
                // Create handles
                CreateHandles(m_CurrentEditInteractable);
                
            }
            else if (m_EditModeManager.CurrentState == EditModeStates.ScalePerAxis)
            {
                // Re-enable grabbing for the current interactable
                if (m_CurrentEditInteractable != null)
                {
                    XRGrabInteractable grabInteractable = m_CurrentEditInteractable.GetComponent<XRGrabInteractable>();
                    if (grabInteractable != null)
                    {
                        grabInteractable.enabled = true;
                    }
                }
                
                m_EditModeManager.ChangeState(EditModeStates.Idle);
                m_CurrentEditInteractable = null;
                
                // Destroy handles
                DestroyHandles();
            }
        }
        
        private void CreateHandles(XRBaseInteractable interactable)
        {
            Renderer targetRenderer = interactable.GetComponent<Renderer>();
            MeshFilter meshFilter = interactable.GetComponent<MeshFilter>();
            if (targetRenderer == null) return;

            Vector3 localExtents;
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                // Use mesh extents for more precise control
                localExtents = meshFilter.sharedMesh.bounds.extents;
            }
            else
            {
                // Fallback to renderer bounds
                localExtents = targetRenderer.bounds.extents;
            }

            // Local positions for handles
            Vector3 localXAxisPosition = localExtents.x * Vector3.right * 0.5f;
            Vector3 localYAxisPosition = localExtents.y * Vector3.up * 0.5f;
            Vector3 localZAxisPosition = localExtents.z * Vector3.forward * 0.5f;

            // Rotate the local positions based on the interactable's rotation
            localXAxisPosition = interactable.transform.rotation * localXAxisPosition;
            localYAxisPosition = interactable.transform.rotation * localYAxisPosition;
            localZAxisPosition = interactable.transform.rotation * localZAxisPosition;

            // Add to center to position in world space
            Vector3 worldXAxisPosition = interactable.transform.position + localXAxisPosition;
            Vector3 worldYAxisPosition = interactable.transform.position + localYAxisPosition;
            Vector3 worldZAxisPosition = interactable.transform.position + localZAxisPosition;

            // Instantiate handles
            xAxisHandle = Instantiate(xAxisHandlePrefab, worldXAxisPosition, Quaternion.identity);
            xAxisHandle.name = "XHandle";
            
            yAxisHandle = Instantiate(yAxisHandlePrefab, worldYAxisPosition, Quaternion.identity);
            yAxisHandle.name = "YHandle";
            
            zAxisHandle = Instantiate(zAxisHandlePrefab, worldZAxisPosition, Quaternion.identity);
            zAxisHandle.name = "ZHandle";
        }


        private void DestroyHandles()
        {
            if (xAxisHandle) Destroy(xAxisHandle);
            if (yAxisHandle) Destroy(yAxisHandle);
            if (zAxisHandle) Destroy(zAxisHandle);
        }
        
        private void HandleScaleHandleGrab(XRBaseInteractable interactable)
        {
            Debug.Log("Grabbed scale handle: " + interactable.gameObject.name);
            // Get the scale axis
            string handleName = interactable.gameObject.name;
            if (handleName == "XHandle")
            {
                m_CurrentScaleAxis = Axis.X;
            }
            else if (handleName == "YHandle")
            {
                m_CurrentScaleAxis = Axis.Y;
            }
            else if (handleName == "ZHandle")
            {
                m_CurrentScaleAxis = Axis.Z;
            }
            else
            {
                m_CurrentScaleAxis = Axis.None;
            }
        }
        
        private void HandleScaleHandleRelease(XRBaseInteractable interactable)
        {
            Debug.Log("Grabbed scale handle: " + interactable.gameObject.name);
            m_CurrentScaleAxis = Axis.None;
            m_prevRightHandPosition = null;
        }

        private void Update()
        {
            if (m_prevRightHandPosition == null)
            {
                m_prevRightHandPosition = rightController.transform.position;
                return;
            }
            switch (m_CurrentScaleAxis)
            {
                // Scale along X-axis, calculate displacement between current right controller position and start grabbing position
                case Axis.X:
                {
                    float displacement = rightController.transform.position.x - m_prevRightHandPosition.Value.x;
                    Vector3 newScale = m_CurrentEditInteractable.transform.localScale;
                    newScale.x += displacement; // Modify scale based on displacement
                    newScale.x = Mathf.Max(0.01f, newScale.x); // Ensure scale is positive
                    m_CurrentEditInteractable.transform.localScale = newScale;
                    break;
                }
                // Scale along Y-axis, calculate displacement between current right controller position and start grabbing position
                case Axis.Y:
                {
                    float displacement = rightController.transform.position.y - m_prevRightHandPosition.Value.y;
                    Vector3 newScale = m_CurrentEditInteractable.transform.localScale;
                    newScale.y += displacement; // Modify scale based on displacement
                    newScale.y = Mathf.Max(0.01f, newScale.y); // Ensure scale is positive
                    m_CurrentEditInteractable.transform.localScale = newScale;
                    break;
                }
                // Scale along Z-axis, calculate displacement between current right controller position and start grabbing position
                case Axis.Z:
                {
                    float displacement = rightController.transform.position.z - m_prevRightHandPosition.Value.z;
                    Vector3 newScale = m_CurrentEditInteractable.transform.localScale;
                    newScale.z += displacement; // Modify scale based on displacement
                    newScale.z = Mathf.Max(0.01f, newScale.z); // Ensure scale is positive
                    m_CurrentEditInteractable.transform.localScale = newScale;
                    break;
                }
                default:
                    break;
            }
            // Update previous right hand position
            m_prevRightHandPosition = rightController.transform.position;
        }
    }
}
