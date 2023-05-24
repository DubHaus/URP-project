using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

namespace Project.Gameplay.UI {
    public class DetailsPanel : VisualElement {

        public enum Tab {
            Games,
            Users
        }

        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<DetailsPanel> { }

        private Button gamesTab => this.Q<Button>("games");
        private Button usersTab => this.Q<Button>("users");
        private Button addButton => this.Q<Button>("add");

        private Tab _activeTab = Tab.Games;

        public Tab activeTab {
            get {
                return _activeTab;
            }
            set {
                if (value == _activeTab) {
                    return;
                } else {
                    _activeTab = value;
                    OnChangeTab(activeTab);
                }
            }
        }


        //// Custom controls need a default constructor. This default constructor 
        //// calls the other constructor in this class.
        //public DetailsPanel() { }

        // Define a constructor that loads the UXML document that defines 
        // the hierarchy of CardElement and assigns an image and badge values.
        public DetailsPanel() {
            // It assumes the UXML file is called "CardElement.uxml" and 
            // is placed at the "Resources" folder.

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/GameScreenUI/DetailsPanel/DetailsPanel.uxml")
                .Completed += result => {
                    if (result.Status == AsyncOperationStatus.Failed) {
                        Debug.LogError("Error while loading DetailsPanelUI");
                    } else {
                        result.Result.CloneTree(this);
                        OnChangeTab(Tab.Games);
                        gamesTab.clicked += () => SwitchTab(Tab.Games);
                        usersTab.clicked += () => SwitchTab(Tab.Games);
                    }
                };
        }

        private void OnChangeTab(Tab activeTab) {

            switch (activeTab) {
                case Tab.Games:
                    this.Q("list").Clear();
                    gamesTab.AddToClassList("active");
                    usersTab.RemoveFromClassList("active");
                    foreach (GameInfo game in GetGamesList()) {
                        // Instantiate a template container.
                        var elem = new GameDetailsCard(game.name, game.status, game.players);
                        // Add the custom element into the scene.
                        this.Q("list").Add(elem);
                    }
                    break;

            }
        }

        private void SwitchTab(Tab tab) {
            activeTab = tab;
        }

        private List<GameInfo> GetGamesList() {
            return new List<GameInfo>
            {
                new GameInfo("Volleyball", "11 : 9",
                    new List<Player>(){
                        new Player("Deaduser"),
                        new Player("Arties121"),
                        new Player("lancelot_de_luac"),
                    }
                ),
                new GameInfo("Chess", "Deaduser turn",
                    new List<Player>(){
                        new Player("Deaduser"),
                        new Player("Arties121"),
                    }
                ),
            };
        }
    }

}