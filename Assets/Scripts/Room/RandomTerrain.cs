using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Room {
    
    // Escolhe um terreno aleatório
    
    public class RandomTerrain : MonoBehaviour {
        [SerializeField] private List<Terrain> terrains;
        [SerializeField] private GameObject waterObject;

        private void Start() {
            Random random = new Random(DateTime.Now.Millisecond);
            int choice = random.Next(terrains.Count);
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