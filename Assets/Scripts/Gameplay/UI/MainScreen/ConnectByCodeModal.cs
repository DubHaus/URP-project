using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Project.Gameplay.UI {
    public class ConnectByCodeModal : VisualElement {

        private TextField CodeField => this.Q<TextField>("codeField");
        private Button ConnectButton => this.Q<Button>("connectButton");

        private Button CloseButton => this.Q<Button>("closeButton");
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<ConnectByCodeModal> {
        }

        public ConnectByCodeModal() { }

        public ConnectByCodeModal(Action<string> connectFn, Action closeFn) {

            Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/MainScreenUI/ConnectByCodeModal/ConnectByCodeModalUI.uxml")
                .Completed += result => {
                if (result.Status == AsyncOperationStatus.Failed) {
                    Debug.LogError("Error while loading ServerCardUI");


                }
                else {
                    result.Result.CloneTree(this);

                    ConnectButton.clicked += () => {
                        if (CodeField.value != "") {
                            connectFn(CodeField.value);
                        }
                    };
                    CloseButton.clicked += closeFn;
                }
            };


        }
    }

}