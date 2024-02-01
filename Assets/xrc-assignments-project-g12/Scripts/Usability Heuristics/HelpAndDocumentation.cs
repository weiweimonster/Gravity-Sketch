using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    
    public class HelpAndDocumentation : MonoBehaviour
    {
        [SerializeField] private GameObject m_HelpButton;
        [SerializeField] private GameObject m_HelpCanvas;
        [SerializeField] private GameObject m_Head;
        [Tooltip("Ray interact event distributor")] [SerializeField]
        private RayInteractorEventDistributor m_RayInteractorEventDistributor;
        
        private bool isHelpAndDocVisible = false;
        private Transform head;
        private float spawnDistance = 1.2f;
        
        private void OnEnable()
        {
            m_HelpCanvas.SetActive(isHelpAndDocVisible); 
        }
        
        public void OnHelpButtonClicked()
        {
            if (m_RayInteractorEventDistributor.State != RayInteractorState.SelectingHelp)
            {
                return;
            }
            
            isHelpAndDocVisible = !isHelpAndDocVisible;
            Debug.Log("Toggle Help and Doc Visibility: " + isHelpAndDocVisible);
            m_HelpCanvas.SetActive(isHelpAndDocVisible);
            
            if (isHelpAndDocVisible)
            {
                // Set Help and Documentation Position
                Transform headTransform = m_Head.transform;
                m_HelpCanvas.transform.position = headTransform.position + new Vector3(headTransform.forward.x, 0, headTransform.forward.z) * spawnDistance;
                m_HelpCanvas.transform.LookAt(headTransform.position);
                m_HelpCanvas.transform.forward *= -1;
            }
        }
        
        public GameObject HelpButton
        {
            get => m_HelpButton;
        } 
    }
}