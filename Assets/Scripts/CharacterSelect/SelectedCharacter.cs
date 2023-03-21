using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacter : MonoBehaviour
{

    [SerializeField] SelectCharacterController.Character character;
    [SerializeField] CameraSystem cameraSystem;

    public bool isSelected
    {
        get
        {
            return SelectCharacterController.Instance.selectedCharacter == character;
        }
    }

    private void Update()
    {
        if (isSelected)
        {
            cameraSystem.Focus(gameObject.transform);
        }
    }
}
