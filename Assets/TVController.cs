using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVController : MonoBehaviour {
    [SerializeField] Color firstColor;
    [SerializeField] Color secondColor;
    [SerializeField] float maxIntensity = 6f;

    private Light lightComponent;
    private float countdown = 0;

    private void Awake() {
        lightComponent = GetComponent<Light>();
    }

    // Start is called before the first frame update
    void Start() {
        lightComponent.color = firstColor;
    }

    // Update is called once per frame
    void Update() {
        lightComponent.intensity = lightComponent.intensity >= maxIntensity ?
            lightComponent.intensity - (5 * Time.deltaTime) : lightComponent.intensity + (5 * Time.deltaTime);
    }
}
