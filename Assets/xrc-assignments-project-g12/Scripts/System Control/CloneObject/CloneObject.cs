using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G12
{
    public class CloneObject : MonoBehaviour
    {
        public void Clone(XRBaseInteractable interactable)
        {
            // Clone the object
            GameObject cloneObject = Instantiate(interactable.gameObject);

            // Get the renderer and material of the cloned object
            MeshRenderer cloneRenderer = cloneObject.GetComponent<MeshRenderer>();
            Material originalMaterial = interactable.GetComponent<MeshRenderer>().material;
            Material cloneMaterial = new Material(originalMaterial);
            cloneRenderer.material = cloneMaterial;
            
            Debug.Log("Clone object");
        }
    }
}