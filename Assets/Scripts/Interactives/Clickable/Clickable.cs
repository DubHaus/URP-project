using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable {
    public event Action<RaycastHit> OnClick;

    public bool enabled = true;

    public void Click(RaycastHit hit) {
        OnClick?.Invoke(hit);
    }
}
