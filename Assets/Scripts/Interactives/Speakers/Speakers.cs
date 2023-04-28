using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Speakers {
    public class Speakers : Interactive.Interactive {
 

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }


        public override void Interact() {
            GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume == 1 ? 0 : 1;
        }
    }

}
