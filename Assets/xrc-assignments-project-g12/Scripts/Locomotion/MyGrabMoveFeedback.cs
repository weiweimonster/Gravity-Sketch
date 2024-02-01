using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace xrc_assignments_project_g12.Scripts.Locomotion
{
    public class MyGrabMoveFeedback : MonoBehaviour
    {
        [SerializeField] private MyGrabMove _grabMove;
        [Tooltip("Left Controller")] [SerializeField]
        private ActionBasedController leftController;

        [Tooltip("Right Controller")] [SerializeField]
        private ActionBasedController rightController;

        [SerializeField] private Canvas canvas;

        [SerializeField] private TextMeshProUGUI text;

        private bool isGrabbing = false;
        private Transform mainCamera;
        private float m_initialFontSize;
        private readonly float[] hapticThresholds = { 25f, 50f, 75f, 100f };
        private bool wasVribating = false;
        
        private void Awake()
        {
            mainCamera = Camera.main.transform;
            text.enabled = false;
            m_initialFontSize = text.fontSize;
        }
        
        private void Update()
        {
            var wasGrabbing = isGrabbing; 
            isGrabbing = _grabMove.rightGrabMoveAction.action.IsPressed() &&
                         _grabMove.leftGrabMoveAction.action.IsPressed();
            if (!wasGrabbing && isGrabbing)
            {
                text.enabled = true;
            }
            text.transform.position =
                (leftController.transform.position + rightController.transform.position) * 0.5f;
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - mainCamera.transform.position, 
                mainCamera.transform.up);
            var currentScale = _grabMove.rig.localScale.x;
            text.text = (100/currentScale).ToString("0") + "%";
            text.fontSize = currentScale * m_initialFontSize;
            if (!isGrabbing)
            {
                text.enabled = false;
            }
            
            // sending haptic feedback
            var localScale = _grabMove.rig.localScale.x;
            var flag = false;
            if ((100f/currentScale) % 25f < 0.5f) 
            { 
                if (!wasVribating) 
                {
                    leftController.SendHapticImpulse(0.5f, 0.1f);
                    rightController.SendHapticImpulse(0.5f, 0.1f); 
                } 
                flag = true;
            }
            
            wasVribating = flag;
        }
    }
}