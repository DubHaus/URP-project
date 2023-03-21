using UnityEngine;


public class SelectCharacterController : MonoBehaviour
{
    public enum Character
    {
        Boy,
        Girl
    }
    public static SelectCharacterController Instance { get; private set; }

    public Character selectedCharacter { get; private set; } = Character.Boy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("More than one SelectCharacterController instance");
        }
        else
        {
            Instance = this;
        }
    }
    
    public void SelectNextCharacter()
    {
        selectedCharacter = selectedCharacter == Character.Boy ? Character.Girl : Character.Boy;
    }
}

