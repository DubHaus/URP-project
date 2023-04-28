using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable {
    public event Action<RaycastHit> OnClick;

    public bool enabled = true;

    public bool Click(RaycastHit hit) {
        if (OnClick != null) {
            OnClick.Invoke(hit);
            return true;
        }
        return false;
    }

    public void ClearSubscribtions() {
        OnClick = null;
    }
}
