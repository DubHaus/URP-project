using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Project.Gameplay.UI {
    public class GameCard : VisualElement {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<GameCard> { }

        private Label nameElem => this.Q<Label>("name");
        private Label playersElem => this.Q<Label>("players");
        private Label statusElem => this.Q<Label>("status");


        // Custom controls need a default constructor. This default constructor 
        // calls the other constructor in this class.
        public GameCard() { }

        // Define a constructor that loads the UXML document that defines 
        // the hierarchy of CardElement and assigns an image and badge values.
        public GameCard(string name, string status, int players) {
            // It assumes the UXML file is called "CardElement.uxml" and 
            // is placed at the "Resources" folder.

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/GameScreenUI/GameCard/GameCard.uxml")
                .Completed += result => {
                    if (result.Status == AsyncOperationStatus.Failed) {
                        Debug.LogError("Error while loading GameCardUI");
                    } else {
                        result.Result.CloneTree(this);

                        nameElem.text = name;
                        playersElem.text = "Players(" + players.ToString() + ")";
                        statusElem.text = status;
                    }
                };
        }
    }

}