using System.Collections;
using System.Collections.Generic;
using Project.Utils.Input;
using UnityEngine;

namespace Project.Interactive {
    public class HighlightController : MonoBehaviour {

        [SerializeField] LayerMask highlightActiveLayers;
        [SerializeField] LayerMask highlightLayer;

        private UnityEngine.Transform? currentHighlight;
        private int currentHighlightLayer;

        // Start is called before the first frame update
        void Start() {
            PlayerInputController.Instance.OnPoint += OnHover;
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnHover(Vector2 screenPosition) {
            float maxDistance = 100;
            if (currentHighlight) {
                currentHighlight.gameObject.layer = currentHighlightLayer;
                currentHighlight = null;
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out RaycastHit hit, maxDistance, highlightActiveLayers)) {
                Debug.Log(hit.transform);
                if (hit.transform.gameObject.TryGetComponent(out Interactive elem)) {
                    if (elem.highlighted.enabled) {
                        currentHighlightLayer = hit.transform.gameObject.layer;
                        hit.transform.gameObject.layer = LayerMask.NameToLayer("Highlight");
                        currentHighlight = hit.transform;
                    }
                }

            }
        }
    }
}

