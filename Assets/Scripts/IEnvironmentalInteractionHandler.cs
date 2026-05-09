using UnityEngine;

public interface IEnvironmentalInteractionHandler
{
    public bool CanIHandleThis(GameObject gameObject);
    public void HandleThis(GameObject gameObject, RaycastHit2D hit, AGJ_CharacterController characterController);
}
