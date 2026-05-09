using UnityEngine;

public class EIH_OrbitHandler : IEnvironmentalInteractionHandler
{
    public bool CanIHandleThis(GameObject go)
    {
        return go.GetComponent<OrbitBox>() != null;
    }

    public void HandleThis(GameObject go, RaycastHit2D hit, AGJ_CharacterController characterController)
    {
        OrbitBox box = go.GetComponent<OrbitBox>();

        characterController.ActivateOrbitingMovement(box.transform);
    }
}
