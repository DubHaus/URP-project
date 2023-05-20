using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Project.UI {
    public class CreateServerModal : VisualElement {

        private TextField ServerNameButton => this.Q<TextField>("serverName");
        private Button CreateButton => this.Q<Button>("createButton");
        private Button CloseButton => this.Q<Button>("closeButton");
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<CreateServerModal> { }

        public CreateServerModal() { }

        public CreateServerModal(Action<string, bool> createServer, Action closeFn) {

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/MainScreenUI/CreateServerModal/CreateServerModal.uxml")
                .Completed += result => {
                    if (result.Status == AsyncOperationStatus.Failed) {
                        Debug.LogError("Error while loading CreateServerModalUI");


                    } else {
                        result.Result.CloneTree(this);

                        CreateButton.clicked += () => {
                            if (ServerNameButton.value != "") {
                                createServer(ServerNameButton.value, false); // TODO add private option in UI
                            }
                        };
                        CloseButton.clicked += closeFn;
                    }
                };


        }
    }

}