using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Project.Gameplay.UI {
    public class ServerCard : VisualElement {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<ServerCard> {
        }

        private VisualElement containerElem => this.Q("container");
        private VisualElement imageElem => this.Q("image");
        private Label lastVisitElem => this.Q<Label>("lastVisit");
        private Label nameElem => this.Q<Label>("name");


        // Custom controls need a default constructor. This default constructor 
        // calls the other constructor in this class.
        public ServerCard() { }

        // Define a constructor that loads the UXML document that defines 
        // the hierarchy of CardElement and assigns an image and badge values.
        public ServerCard(string name) {
            // It assumes the UXML file is called "CardElement.uxml" and 
            // is placed at the "Resources" folder.

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/MainScreenUI/ServerCard/ServerCardUI.uxml")
                .Completed += result => {
                if (result.Status == AsyncOperationStatus.Failed) {
                    Debug.LogError("Error while loading ServerCardUI");
                }
                else {
                    result.Result.CloneTree(this);

                    containerElem.style.backgroundColor = Color.red;
                    containerElem.style.color = Color.black;

                    lastVisitElem.text = "Yesterday";
                    nameElem.text = name;
                }
            };


        }
    }

}