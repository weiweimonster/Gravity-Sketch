using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class TargetProvider : MonoBehaviour
    {
        [SerializeField] private XRDirectInteractor interactor;
        
        private XRBaseInteractable currentTarget = null;
        private Vector3 originalTargetPosition = Vector3.zero;
            
        private void Awake()
        {
            interactor.selectEntered.AddListener(OnObjectSelected);
            interactor.selectExited.AddListener(OnObjectReleased);
        }

        private void OnObjectSelected(SelectEnterEventArgs args)
        {
            Debug.Log("Object Selected");
            currentTarget = args.interactable;
            originalTargetPosition = args.interactable.transform.position;
        }
        
        private void OnObjectReleased(SelectExitEventArgs args)
        {
            Debug.Log("Object Released");
            currentTarget = null;
            originalTargetPosition = Vector3.zero;
        }
        
        public XRBaseInteractable CurrentTarget
        {
            get => currentTarget;
        }
        
        public Vector3 OriginalTargetPosition
        {
            get => originalTargetPosition;
        } 
    }
}