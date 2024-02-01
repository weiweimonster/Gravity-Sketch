using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class ObjectCreation : MonoBehaviour
    {
        [Tooltip("Left Controller")] [SerializeField]
        private ActionBasedController m_LeftController;

        [Tooltip("Right Controller")] [SerializeField]
        private ActionBasedController m_RightController;

        [Tooltip("Cube Prefab")] [SerializeField]
        private GameObject m_CubePrefab;
        [Tooltip("Sphere Prefab")] [SerializeField]
        private GameObject m_SpherePrefab;
        [Tooltip("Cylinder Prefab")] [SerializeField]
        private GameObject m_CylinderPrefab;
        [Tooltip("Capsule Prefab")] [SerializeField]
        private GameObject m_CapsulePrefab;
        
        
        [Tooltip("Ray interact event distributor")] [SerializeField]
        private RayInteractorEventDistributor m_RayInteractorEventDistributor;

        private float distance = 0f;
        private Vector3 center;
        private GameObject m_CurrentShape;

        private void Awake()
        {
            // Set default shape to cube
            m_CurrentShape = m_CubePrefab;
        }

        private void Start()
        {
            if (m_RayInteractorEventDistributor != null)
            {
                m_RayInteractorEventDistributor.OnShapeChanged += _UpdateObjectShape;
            }
        }
        
        private void OnDestroy()
        {
            if (m_RayInteractorEventDistributor != null)
            {
                m_RayInteractorEventDistributor.OnShapeChanged -= _UpdateObjectShape;
            }
        }

        // ------------------------ Private Methods ------------------------
        private void _UpdateDistanceBetweenControllers()
        {
            Vector3 rightControllerPosition = m_RightController.transform.position;
            Vector3 leftControllerPosition = m_LeftController.transform.position;
            center = (rightControllerPosition + leftControllerPosition) / 2;
            distance = Vector3.Distance(rightControllerPosition, leftControllerPosition);
        }
        
        private void _CreateObject()
        {
            GameObject newCube = Instantiate(m_CurrentShape, center, Quaternion.identity);
            newCube.transform.localScale = new Vector3(distance, distance, distance);
            distance = 0;
        }
        
        private void _UpdateObjectShape(GameObject shape)
        {
            Debug.Log("Update Object Shape: " + shape.transform.gameObject.name);
            switch (shape.transform.gameObject.name)
            {
                case "Cube":
                    m_CurrentShape = m_CubePrefab;
                    break;
                case "Sphere":
                    m_CurrentShape = m_SpherePrefab;
                    break;
                case "Cylinder":
                    m_CurrentShape = m_CylinderPrefab;
                    break;
                case "Capsule":
                    m_CurrentShape = m_CapsulePrefab;
                    break;
                default:
                    break;
            }
        }

        // ------------------------ Public Methods ------------------------
        public float Distance
        {
            get => distance;
        }
        
        public Vector3 Center
        {
            get => center;
        }
        
        public void UpdateDistanceBetweenControllers()
        {
            _UpdateDistanceBetweenControllers();
        }
        
        public void CreateObject()
        {
            _CreateObject();
        }
    }
}
