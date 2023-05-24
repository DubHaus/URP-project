using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

namespace Project.Gameplay.UI {
    public class Profile : VisualElement {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<Profile> { }

        private Label nameElem => this.Q<Label>("name");
        private Label initialsElem => this.Q<Label>("initials");

        // Custom controls need a default constructor. This default constructor 
        // calls the other constructor in this class.
        public Profile() { }

        // Define a constructor that loads the UXML document that defines 
        // the hierarchy of CardElement and assigns an image and badge values.
        public Profile(string name) {
            // It assumes the UXML file is called "CardElement.uxml" and 
            // is placed at the "Resources" folder.

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/Components/Profile/Profile.uxml")
                .Completed += result => {
                    if (result.Status == AsyncOperationStatus.Failed) {
                        Debug.LogError("Error while loading GameCardUI");
                    } else {
                        result.Result.CloneTree(this);
                        Debug.Log("NAME " + name);
                        nameElem.text = name;
                        initialsElem.text = "" + name.First() + name.Last();
                    }
                };
        }
    }

}