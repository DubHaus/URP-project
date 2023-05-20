using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

namespace Project.UI {
    public class GameDetailsCard : VisualElement {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<GameDetailsCard> { }

        private Label nameElem => this.Q<Label>("name");
        private Label playersTextElem => this.Q<Label>("playersText");
        private Label statusElem => this.Q<Label>("status");


        // Custom controls need a default constructor. This default constructor 
        // calls the other constructor in this class.
        public GameDetailsCard() { }

        // Define a constructor that loads the UXML document that defines 
        // the hierarchy of CardElement and assigns an image and badge values.
        public GameDetailsCard(string name, string status, List<Player> players) {
            // It assumes the UXML file is called "CardElement.uxml" and 
            // is placed at the "Resources" folder.

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/GameScreenUI/DetailsPanel/GameDetailsCard.uxml")
                .Completed += result => {
                    if (result.Status == AsyncOperationStatus.Failed) {
                        Debug.LogError("Error while loading GameCardUI");
                    } else {
                        result.Result.CloneTree(this);

                        RenderUpdate(name, status, players);
                    }
                };
        }


        private void RenderUpdate(string name, string status, List<Player> players) {
            nameElem.text = name;
            playersTextElem.text = "Players(" + players.Count + ")";
            statusElem.text = status;

            players.ForEach(player => {
                var elem = new Profile(player.name);
                // Add the custom element into the scene.
                this.Q("playerList").Add(elem);
            });
        }
    }

}