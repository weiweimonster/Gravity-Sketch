using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class SetShapeFeedback : MonoBehaviour
    {
        [SerializeField] private Color m_SelectColor;
        [SerializeField] private SetShape m_SetShape;
        [SerializeField] private RayInteractorEventDistributor m_RayInteractorEventDistributor;

        private GameObject currentlySelectedShape;
        private Dictionary<GameObject, Color> m_OriginalColor = new Dictionary<GameObject, Color>();

        private void Awake()
        {
            // Initialize the selected shape (Cube) with select color
            foreach (var shape in m_SetShape.ShapePrefabs)
            {
                if (shape != null)
                {
                    // Store the original color
                    m_OriginalColor[shape] = shape.GetComponent<MeshRenderer>().material.color;

                    // Set Cube as the initially selected shape
                    if (shape.transform.gameObject.name == "Cube")
                    {
                        currentlySelectedShape = shape;
                        shape.GetComponent<MeshRenderer>().material.color = m_SelectColor;
                    }
                }
            }
        }

        private void Start()
        {
            if (m_RayInteractorEventDistributor != null)
            {
                m_RayInteractorEventDistributor.OnShapeChanged += _UpdateSelected;
            }
        }

        private void OnDestroy()
        {
            if (m_RayInteractorEventDistributor != null)
            {
                m_RayInteractorEventDistributor.OnShapeChanged -= _UpdateSelected;
            }
        }

        private void _UpdateSelected(GameObject newShape)
        {
            // Reset the color of the currently selected shape
            if (currentlySelectedShape != null)
            {
                currentlySelectedShape.GetComponent<MeshRenderer>().material.color = m_OriginalColor[currentlySelectedShape];
            }

            // Update the current selection
            currentlySelectedShape = newShape;

            // Apply the selection color to the new shape
            if (newShape != null)
            {
                newShape.GetComponent<MeshRenderer>().material.color = m_SelectColor;
            }
        }
    }
}
