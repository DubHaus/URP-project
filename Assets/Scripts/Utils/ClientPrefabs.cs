using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utils {

    public class ClientPrefabs {
        const string k_ClientGUIDKey = "client_guid";

        public static string GetGuid() {
            if (PlayerPrefs.HasKey(k_ClientGUIDKey)) {
                return PlayerPrefs.GetString(k_ClientGUIDKey);
            }

            var guid = System.Guid.NewGuid();
            var guidString = guid.ToString();
            PlayerPrefs.SetString(k_ClientGUIDKey, guidString);

            return guidString;
        }
    }
}

