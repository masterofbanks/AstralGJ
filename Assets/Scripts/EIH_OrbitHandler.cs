using System.Collections.Generic;
using UnityEngine;

public class EIH_OrbitHandler : IEnvironmentalInteractionHandler
{
    public bool CanIHandleThis(GameObject go)
    {
        return go.GetComponent<OrbitBox>() != null;
    }

    public bool CanIHandleThis(List<Collider2D> colliders, out GameObject whatICanHandle)
    {
        foreach (Collider2D collider in colliders)
        {
            if (CanIHandleThis(collider.gameObject))
            {
                whatICanHandle = collider.gameObject;
                return true;
            }
        }

        whatICanHandle = null;
        return false;
    }

    public void HandleThis(GameObject go, AGJ_CharacterController characterController)
    {
        OrbitBox box = go.GetComponent<OrbitBox>();

        characterController.ActivateOrbitingMovement(box.transform);
    }
}
