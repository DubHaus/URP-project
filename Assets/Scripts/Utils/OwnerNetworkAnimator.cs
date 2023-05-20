using UnityEngine;
using Unity.Netcode.Components;

namespace  Project.Utils {
    public class OwnerNetworkAnimator : NetworkAnimator {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
    
}
