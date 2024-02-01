using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRC.Assignments.Project.G12
{
    public class SetColorFeedback : MonoBehaviour
    {
        
        [SerializeField] private SetColor m_SetColor;
        [SerializeField] private TMPro.TextMeshProUGUI m_RedValueDisplay;
        [SerializeField] private TMPro.TextMeshProUGUI m_GreenValueDisplay;
        [SerializeField] private TMPro.TextMeshProUGUI m_BlueValueDisplay;
        
        // Start is called before the first frame update
        void Start()
        {
            if (m_SetColor != null)
            {
                m_SetColor.OnRedColorChanged += UpdateRedValueDisplay;
                m_SetColor.OnGreenColorChanged += UpdateGreenValueDisplay;
                m_SetColor.OnBlueColorChanged += UpdateBlueValueDisplay;
            }
        }
        
        private void OnDestroy()
        {
            if (m_SetColor != null)
            {
                m_SetColor.OnRedColorChanged -= UpdateRedValueDisplay;
                m_SetColor.OnGreenColorChanged -= UpdateGreenValueDisplay;
                m_SetColor.OnBlueColorChanged -= UpdateBlueValueDisplay;
            }
        }

        private void UpdateRedValueDisplay(float value)
        {
            float displayValue = value * 255f;
            m_RedValueDisplay.text = displayValue.ToString("0");
        }
        
        private void UpdateGreenValueDisplay(float value)
        {
            float displayValue = value * 255f;
            m_GreenValueDisplay.text = displayValue.ToString("0");
        }
        
        private void UpdateBlueValueDisplay(float value)
        {
            float displayValue = value * 255f;
            m_BlueValueDisplay.text = displayValue.ToString("0");
        }
    }
}
