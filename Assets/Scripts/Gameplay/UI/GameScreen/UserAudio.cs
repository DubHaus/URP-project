using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Project.Gameplay.UI {
    public class UserAudio : VisualElement {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<UserAudio> {
        }

        private VisualElement microElem => this.Q("micro");
        private VisualElement audioElem => this.Q("audio");
        private Label usernameElem => this.Q<Label>("username");


        // Custom controls need a default constructor. This default constructor 
        // calls the other constructor in this class.
        public UserAudio() { }

        // Define a constructor that loads the UXML document that defines 
        // the hierarchy of CardElement and assigns an image and badge values.
        public UserAudio(string username, string name, bool microOn, bool audioOn) {
            // It assumes the UXML file is called "CardElement.uxml" and 
            // is placed at the "Resources" folder.

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/GameScreenUI/UserAudio/UserAudio.uxml")
                .Completed += result => {
                if (result.Status == AsyncOperationStatus.Failed) {
                    Debug.LogError("Error while loading UserAuidoUI");
                }
                else {
                    this.name = name;
                    result.Result.CloneTree(this);

                    usernameElem.text = username;
                    SetAudio(audioOn);
                    SetMicro(microOn);
                }
            };
        }

        public void SetAudio(bool active) {
            audioElem.AddToClassList(active ? "headsetIcon" : "headsetOffIcon");
            audioElem.RemoveFromClassList(active ? "headsetOffIcon" : "headsetIcon");
        }

        public void SetMicro(bool active) {
            microElem.AddToClassList(active ? "microIcon" : "microOffIcon");
            microElem.RemoveFromClassList(active ? "microOffIcon" : "microIcon");
        }
    }

}