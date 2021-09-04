using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;

namespace Room {
    
    // Escolhe um terreno aleatório
    public class RandomTerrain : MonoBehaviour {
        [SerializeField] private List<Terrain> terrains;
        [SerializeField] private GameObject waterObject;

        private void Start() {
            StartCoroutine(setMap());
        }

        private IEnumerator setMap() {
            Task<DataSnapshot> task = DatabaseAPI.getDatabase().Child("rooms").Child(PlayerPrefs.GetString("room", null)).Child("map").GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted);

            int choice = int.Parse(task.Result.Value.ToString());
            if (choice != 1) {
                waterObject.SetActive(false);
            }
            terrains[choice].gameObject.layer = LayerMask.NameToLayer("TerrainActive");
            for (int i = 0; i < terrains.Count; i++) {
                if (i != choice) {
                    terrains[i].gameObject.SetActive(false);
                }
            }
        }
    }
}