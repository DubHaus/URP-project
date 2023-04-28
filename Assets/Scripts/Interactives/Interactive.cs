using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace Project.Interactive {

    public class Interactive : NetworkBehaviour {
        public virtual void Interact() { }

        public Clickable clickable = new Clickable();
        public Highlighted highlighted = new Highlighted();

        public override void OnDestroy() {
            base.OnDestroy();
            clickable.ClearSubscribtions();
        }

    }


    //public class NetworkInteractive : NetworkBehaviour {
    //    public virtual void Interact() { }

    //    public Clickable clickable = new Clickable();
    //    public Highlighted highlighted = new Highlighted();

    //}
}
