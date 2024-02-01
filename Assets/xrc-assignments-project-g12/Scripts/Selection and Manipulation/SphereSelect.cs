using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class SphereSelect : MonoBehaviour
    {
        [SerializeField] private XRDirectInteractor interactor;
        [SerializeField] private List<XRBaseInteractable> m_PreventSelectingInteractables;
        [SerializeField] private EditModeManager m_EditModeManager;
        
        
        
        private List<XRBaseInteractable> m_CollideInteractables = new List<XRBaseInteractable>();
        private List<XRBaseInteractable> m_GrabbedInteractables = new List<XRBaseInteractable>();
        private List<Transform> m_OriginalTransform = new List<Transform>();
        private List<GameObject> m_AttachTransforms = new List<GameObject>();
        private ActionBasedController m_controller;
        private bool m_isGrabbing;
        
        private readonly float m_MinRadius = 0.03f;
        private readonly float m_MaxRadius = 0.5f;

        // events for feedback script to subscribe
        public event Action<XRBaseInteractable> onHoverEnter;
        public event Action<XRBaseInteractable> onHoverExit;
        public event Action<XRBaseInteractable> onSelectEnter;
        public event Action<XRBaseInteractable> onSelectExit;

        // event for scale axis
        public event Action<XRBaseInteractable> onScaleHandleGrab;
        public event Action<XRBaseInteractable> onScaleHandleRelease;
        
        private void Awake()
        {
            interactor.selectEntered.AddListener(OnSphereSelecting);
            interactor.selectExited.AddListener(OnSphereExiting);
            interactor.hoverEntered.AddListener(OnAddCollidedInteractable);
            interactor.hoverExited.AddListener(OnRemoveCollidedInteractable);
            m_controller = GetComponentInParent<ActionBasedController>();
            if (m_controller == null) Debug.Log("controller is null");
        }

        
        private void OnAddCollidedInteractable(HoverEnterEventArgs args)
        {
            // check if the interactable is in m_PreventSelectingInteractables
            // if yes, no-op
            if (m_PreventSelectingInteractables.Contains((XRBaseInteractable)args.interactableObject))
            {
                return;
            }
            if (!m_CollideInteractables.Contains((XRBaseInteractable)args.interactableObject))
            {
                XRBaseInteractable interactable = (XRBaseInteractable)args.interactableObject;
                m_CollideInteractables.Add(interactable);
                Debug.Log("add one collided interactable");
                
                // invoke event and pass interactable
                onHoverEnter?.Invoke(interactable);
            }
        }

        private void OnRemoveCollidedInteractable(HoverExitEventArgs args)
        {
            if (m_CollideInteractables.Contains((XRBaseInteractable)args.interactableObject))
            {
                XRBaseInteractable interactable = (XRBaseInteractable)args.interactableObject;
                m_CollideInteractables.Remove(interactable);
                Debug.Log("remove one collided interactable");
                
                // invoke event and pass interactable
                onHoverExit?.Invoke(interactable);
            }
        }
    

        private void OnSphereSelecting(SelectEnterEventArgs args)
        {
            // if edit mode is in scale per axis
            if (m_EditModeManager.CurrentState == EditModeStates.ScalePerAxis)
            {
                // invoke certain event  
                onScaleHandleGrab?.Invoke((XRBaseInteractable)args.interactableObject);
            }
            
            // check if the interactable is in m_PreventSelectingInteractables
            // if yes, no-op
            if (m_PreventSelectingInteractables.Contains((XRBaseInteractable)args.interactableObject))
            {
                return;
            }
            
            var attachTransform = args.interactorObject.GetAttachTransform(args.interactableObject);
            attachTransform.position = args.interactableObject.transform.position;
            attachTransform.rotation = args.interactableObject.transform.rotation;

            if (m_CollideInteractables.Count > 0)
            {
                foreach (var interactable in m_CollideInteractables)
                {
                    CreateAttachTransform(interactable);
                    m_OriginalTransform.Add(interactable.transform.parent);
                    m_GrabbedInteractables.Add(interactable);
                    Debug.Log("add 1 sphere select this should only appear once right now");
                    
                    // invoke event and pass interactable
                    onSelectEnter?.Invoke(interactable);
                }
            }
            
            Debug.Log("sphere selecting");
            m_isGrabbing = true;
        }
    
        private void OnSphereExiting(SelectExitEventArgs args)
        {
            // if edit mode is in scale per axis
            if (m_EditModeManager.CurrentState == EditModeStates.ScalePerAxis)
            {
                // invoke certain event  
                onScaleHandleRelease?.Invoke((XRBaseInteractable)args.interactableObject);
            }
            
            if (m_GrabbedInteractables.Count > 0)
            {
                for (int i = 0; i < m_GrabbedInteractables.Count; i++)
                {
                    var interactable = m_GrabbedInteractables[i];
                    interactable.transform.parent = m_OriginalTransform[i];
                }
            }
            
            DestroyAttachTransform();
            m_AttachTransforms.Clear();
            m_GrabbedInteractables.Clear();
            
            foreach (var interactable in m_CollideInteractables)
            {
                onSelectExit?.Invoke(interactable);
            }
            
            Debug.Log("exiting sphere select");
            m_isGrabbing = false;
        }
    
        private void CreateAttachTransform(XRBaseInteractable interactable)
        {
            GameObject createdAttachTransform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            createdAttachTransform.transform.position = interactable.transform.position;
            createdAttachTransform.transform.rotation = interactable.transform.rotation;
            createdAttachTransform.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            createdAttachTransform.transform.parent = this.transform;
            m_AttachTransforms.Add(createdAttachTransform);
        }
        private void DestroyAttachTransform()
        {
            foreach (var attachTransform in m_AttachTransforms)
            {
                Destroy(attachTransform);
            }
        }
        
        // ---------------- public methods ----------------
        public bool IsGrabbing()
        {
            return m_isGrabbing;
        }

        public void DeleteGrabbedInteractables()
        {
            List<XRBaseInteractable> objectsToDelete = new List<XRBaseInteractable>(m_GrabbedInteractables);
            DestroyAttachTransform();
            m_AttachTransforms.Clear();
            m_GrabbedInteractables.Clear();
            foreach (var interactable in objectsToDelete)
            {
                if (interactable != null)
                {
                    GameObject target = interactable.gameObject;
                    Destroy(target);
                }
            }
        }

        public void Grab()
        {
            for (int i = 0; i < m_GrabbedInteractables.Count; i++)
            {
                var interactable = m_GrabbedInteractables[i];
                var attachTransform = m_AttachTransforms[i];
                Transform interactableTransform = interactable.transform;
                interactableTransform.position = attachTransform.transform.position;
                interactableTransform.rotation = attachTransform.transform.rotation;
            }
        }
        
        public void SetRadius(float radius)
        {
            // clamp
            float newRadius = Mathf.Clamp(radius, m_MinRadius, m_MaxRadius);
            gameObject.transform.localScale = new Vector3(newRadius, newRadius, newRadius);
        }

    }
}
