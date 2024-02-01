using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRC.Assignments.Project.G12
{
    public class EditModeManager : MonoBehaviour
    {
        private EditModeStates m_CurrentState = EditModeStates.Idle;
        
        public EditModeStates CurrentState => m_CurrentState;
        
        public void ChangeState(EditModeStates state)
        {
            m_CurrentState = state;
        }
    }
}
