using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour {
    public event Action<RaycastHit> OnClick;


    public void Click(RaycastHit hit) {
        OnClick?.Invoke(hit);
    }
}
