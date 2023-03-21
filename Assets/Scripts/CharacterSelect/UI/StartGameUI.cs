using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUI : MonoBehaviour
{

    [SerializeField] Button startGameButton;
    [SerializeField] Button changeCharacterButton;

    private void Awake() {
        startGameButton.onClick.AddListener(() => {
            Debug.Log("START GAME : " + SelectCharacterController.Instance.selectedCharacter);
        });
        
        changeCharacterButton.onClick.AddListener(() => {
            SelectCharacterController.Instance.SelectNextCharacter();
        });
    }
}
