using System.Collections.Generic;
using UnityEngine;

public interface IEnvironmentalInteractionHandler
{
    public bool CanIHandleThis(GameObject gameObject);
    public bool CanIHandleThis(List<Collider2D> colliders, out GameObject whatICanHandle);
    public void HandleThis(GameObject gameObject, AGJ_CharacterController characterController);
}
