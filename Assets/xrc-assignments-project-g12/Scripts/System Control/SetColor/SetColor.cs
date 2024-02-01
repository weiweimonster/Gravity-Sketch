using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;


namespace XRC.Assignments.Project.G12
{
    public class SetColor : MonoBehaviour
    {
        [SerializeField] private XRDirectInteractor interactor;
        [SerializeField] private GameObject m_ColorPicker;
        [SerializeField] private GameObject m_Head;
        [SerializeField] private TargetProvider m_TargetProvider;
        
        [SerializeField] private Slider m_RedSlider;
        [SerializeField] private Slider m_GreenSlider;
        [SerializeField] private Slider m_BlueSlider;
        
        private Dictionary<XRBaseInteractable, Color> m_InteractableColor = new Dictionary<XRBaseInteractable, Color>();
        
        // Color Picker Feedback Event
        public event Action<float> OnRedColorChanged;
        public event Action<float> OnGreenColorChanged;
        public event Action<float> OnBlueColorChanged;
        
        // Ray Interactor State Event
        public event Action OnColorPickerActivated;
        public event Action OnColorPickerDeactivated;
        
        private bool isColorPickerVisible = false;
        private float spawnDistance = 1.2f;
        
        private void Awake()
        {
            interactor.selectExited.AddListener(HideColorPicker); 
        }

        private void OnEnable()
        {
            m_ColorPicker.SetActive(isColorPickerVisible); 
        }

        private void HideColorPicker(SelectExitEventArgs args)
        {
            
            isColorPickerVisible = false;
            m_ColorPicker.SetActive(isColorPickerVisible);
            OnColorPickerDeactivated?.Invoke();
        }
        
        
        private void ChangeSelectedObjectsColor()
        {
            Color newColor = new Color(m_RedSlider.value, m_GreenSlider.value, m_BlueSlider.value);
            
            XRBaseInteractable interactable = m_TargetProvider.CurrentTarget;
            Transform interactableTransform = interactable.transform;
            interactableTransform.GetComponent<Renderer>().material.color = newColor;
            m_InteractableColor[interactable] = newColor;
        }
        
        // =================== Public Methods ===================
        public void ToggleColorPickerVisibility()
        {
            
            if (!m_TargetProvider.CurrentTarget)
            {
                return;
            }
            
            isColorPickerVisible = !isColorPickerVisible;
            Debug.Log("Toggle Color Picker Visibility: " + isColorPickerVisible);
            m_ColorPicker.SetActive(isColorPickerVisible);
            
            if (isColorPickerVisible)
            {
                // Set Color Picker Position
                Transform headTransform = m_Head.transform;
                m_ColorPicker.transform.position = headTransform.position + new Vector3(headTransform.forward.x, 0, headTransform.forward.z) * spawnDistance; 
                m_ColorPicker.transform.LookAt(headTransform.position);
                m_ColorPicker.transform.forward *= -1;
                
                // Initialize Color Picker 
                XRBaseInteractable interactable = m_TargetProvider.CurrentTarget;
                Color color = interactable.GetComponent<Renderer>().material.color;
                m_RedSlider.value = color.r;
                m_GreenSlider.value = color.g;
                m_BlueSlider.value = color.b;
            }
        
            if (isColorPickerVisible)
            {
                OnColorPickerActivated?.Invoke();
            }
            else
            {
                OnColorPickerDeactivated?.Invoke();
            }
        }
        
        public void OnRedSliderValueChanged(float value)
        {
            Debug.Log("Red Slider Value Changed: " + value);
            ChangeSelectedObjectsColor();
            OnRedColorChanged?.Invoke(value);
        }
        
        public void OnGreenSliderValueChanged(float value)
        {
            Debug.Log("Green Slider Value Changed: " + value);
            ChangeSelectedObjectsColor();
            OnGreenColorChanged?.Invoke(value);
        }
        
        public void OnBlueSliderValueChanged(float value)
        {
            Debug.Log("Blue Slider Value Changed: " + value);
            ChangeSelectedObjectsColor();
            OnBlueColorChanged?.Invoke(value);
        }
        
        public void AddInteractableColor(XRBaseInteractable interactable, Color color)
        {
            m_InteractableColor.Add(interactable, color);
        }
        
        public void RemoveInteractableColor(XRBaseInteractable interactable)
        {
            m_InteractableColor.Remove(interactable);
        }

        public void RecoverInteractableColor(XRBaseInteractable interactable)
        {
            Color color = m_InteractableColor[interactable];
            interactable.GetComponent<Renderer>().material.color = color;
        }
    }
}
