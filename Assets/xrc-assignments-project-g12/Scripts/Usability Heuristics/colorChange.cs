using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace xrc_assignments_project_g12.Scripts.Usability_Heuristics
{
    public class colorChange: MonoBehaviour
    {
        [SerializeField] private GameObject controllerPrefab;
        [SerializeField] private Color color;
        
        Color originalColor;
        
        private void Awake()
        {
            originalColor = controllerPrefab.GetComponent<Renderer>().material.color;
        }
        
        public void ButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ColorChange();
            }
            else
            {
                ResetColor();
            }
        }

        private void ColorChange()
        {
            var meshRenderer = controllerPrefab.GetComponent<MeshRenderer>();
            meshRenderer.material.SetColor("_Color", color);
        }
        
        private void ResetColor()
        {
            var meshRenderer = controllerPrefab.GetComponent<Renderer>();
            meshRenderer.material.SetColor("_Color", originalColor);
        }
    }
}