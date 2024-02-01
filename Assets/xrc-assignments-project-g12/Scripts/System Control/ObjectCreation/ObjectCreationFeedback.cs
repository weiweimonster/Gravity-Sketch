using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XRC.Assignments.Project.G12;

namespace XRC.Assignments.Project.G12
{
    public class ObjectCreationFeedback : MonoBehaviour
    {
        [Tooltip("Object Creator")] [SerializeField]
        private ObjectCreation m_ObjectCreation;
        [Tooltip("Ray interact event distributor")] [SerializeField]
        private RayInteractorEventDistributor m_RayInteractorEventDistributor;

        [Tooltip("Visualizer Cube Prefab")] [SerializeField]
        private GameObject m_VisualizerCubePrefab;
        [Tooltip("Visualizer Sphere Prefab")] [SerializeField]
        private GameObject m_VisualizerSpherePrefab;
        [Tooltip("Visualizer Capsule Prefab")] [SerializeField]
        private GameObject m_VisualizerCapsulePrefab;
        [Tooltip("Visualizer Cylinder Prefab")] [SerializeField]
        private GameObject m_VisualizerCylinderPrefab;

        [Tooltip("Visualizer Instance")] [SerializeField]
        private GameObject m_VisualizerInstance;

        private GameObject m_VisualizerPrefab;
        

        private void _UpdateVisualizer(GameObject newShape)
        {
            Debug.Log("Updating visualizer to new shape: " + newShape.name);

            if (m_VisualizerInstance != null)
            {
                Destroy(m_VisualizerInstance);
            }

            switch (newShape.name)
            {
                case "Cube":
                    m_VisualizerInstance = Instantiate(m_VisualizerCubePrefab);
                    break;
                case "Sphere":
                    m_VisualizerInstance = Instantiate(m_VisualizerSpherePrefab);
                    break;
                case "Cylinder":
                    m_VisualizerInstance = Instantiate(m_VisualizerCylinderPrefab);
                    break;
                case "Capsule":
                    m_VisualizerInstance = Instantiate(m_VisualizerCapsulePrefab);
                    break;
                default:
                    Debug.LogWarning("Shape not recognized: " + newShape.name);
                    return;
            }

            m_VisualizerInstance.SetActive(false);
        }
        
        // Start is called before the first frame update
        void Awake()
        {
            // Set default shape to cube
            if (m_VisualizerCubePrefab != null)
            {
                m_VisualizerInstance = Instantiate(m_VisualizerCubePrefab);
                m_VisualizerInstance.SetActive(false);
            }
        }
        
        private void Start()
        {
            if (m_RayInteractorEventDistributor != null)
            {
                m_RayInteractorEventDistributor.OnShapeChanged += _UpdateVisualizer;
            }
        }
        
        private void OnDestroy()
        {
            if (m_RayInteractorEventDistributor != null)
            {
                m_RayInteractorEventDistributor.OnShapeChanged -= _UpdateVisualizer;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_ObjectCreation.Distance > 0)
            {
                if (!m_VisualizerInstance.activeSelf)
                {
                    m_VisualizerInstance.SetActive(true);
                }

                m_VisualizerInstance.transform.position = m_ObjectCreation.Center;
                m_VisualizerInstance.transform.localScale = new Vector3(m_ObjectCreation.Distance,
                    m_ObjectCreation.Distance, m_ObjectCreation.Distance);
            }
            else
            {
                if (m_VisualizerInstance.activeSelf)
                {
                    m_VisualizerInstance.SetActive(false);
                }
            }
        }
    }
}