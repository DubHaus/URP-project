using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SelectCharacterAnimatior : MonoBehaviour
{

    private const string IS_SELECTED = "IsSelected";

    [SerializeField] private SelectedCharacter character;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(IS_SELECTED, character.isSelected);
    }
}
