using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utils.Input;
using Project.Gameplay.GameplayObjects.Character;
using Project.Interactive;

public class ClickableController : MonoBehaviour {

    [SerializeField] LayerMask clickableLayers;
    // Start is called before the first frame update
    void Start() {
        PlayerInputController.Instance.OnClick2D += OnClick;
    }


    private void OnDestroy() {
        PlayerInputController.Instance.OnClick2D -= OnClick;
    }

    private void OnClick(Vector2 clickPosition) {
        float maxDistance = 100;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(clickPosition), out RaycastHit hit, maxDistance, clickableLayers)) {
            if (hit.transform.gameObject.TryGetComponent(out Clickable elem)) {
                elem.Click(hit);
            }
        }
    }
}
