using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay;
using Unity.Netcode;
using VContainer;

namespace Project.Gameplay.GameplayObjects {

    public class PlayerSpawner : MonoBehaviour {

        [SerializeField] private GameObject boy; //add prefab in inspector
        [SerializeField] private GameObject girl; //add prefab in inspector

        public void SpawnPlayer(ulong clientId, GameState.Character character) {
            GameObject newPlayer = Instantiate(character == GameState.Character.Boy ? boy : girl);
            newPlayer.SetActive(true);
            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

}
