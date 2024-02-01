using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class SphereSelectFeedback : MonoBehaviour
    {
        [SerializeField] private Color hoverColor;
        [SerializeField] private SphereSelect m_SphereSelect;
        [SerializeField] private SetColor m_SetColor;
        
        private void Start()
        {
            if (m_SphereSelect != null)
            {
                m_SphereSelect.onHoverEnter += HandleHoverEnter;
                m_SphereSelect.onHoverExit += HandleHoverExit;
                m_SphereSelect.onSelectEnter += HandleSelectEnter;
                m_SphereSelect.onSelectExit += HandleSelectExit;
            }
        }

        private void OnDestroy()
        {
            if (m_SphereSelect != null)
            {
                m_SphereSelect.onHoverEnter -= HandleHoverEnter;
                m_SphereSelect.onHoverExit -= HandleHoverExit;
                m_SphereSelect.onSelectEnter -= HandleSelectEnter;
                m_SphereSelect.onSelectExit -= HandleSelectExit;
            }
        }
        
        private void HandleHoverEnter(XRBaseInteractable interactable)
        {
            // Save original color and change to hover color
            Color originalColor = interactable.GetComponent<MeshRenderer>().material.color;
            m_SetColor.AddInteractableColor(interactable, originalColor);
            interactable.GetComponent<MeshRenderer>().material.color = hoverColor;
        }

        private void HandleHoverExit(XRBaseInteractable interactable)
        {
            // Recover original color
            m_SetColor.RecoverInteractableColor(interactable);
            m_SetColor.RemoveInteractableColor(interactable);
        }
        
        private void HandleSelectEnter(XRBaseInteractable interactable)
        {
            // Recover original color Temporarily
            m_SetColor.RecoverInteractableColor(interactable); 
        }
        
        private void HandleSelectExit(XRBaseInteractable interactable)
        {
            // Add back hover color
            interactable.GetComponent<MeshRenderer>().material.color = hoverColor;
        }
    }
}
