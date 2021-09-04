using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Room {
    public class RandomTerrain : MonoBehaviour {
        [SerializeField] private List<Terrain> terrains;

        private void Start() {
            Random random = new Random(DateTime.Now.Millisecond);
            int choice = random.Next(terrains.Count);
            terrains[choice].gameObject.layer = LayerMask.NameToLayer("TerrainActive");
            for (int i = 0; i < terrains.Count; i++) {
                if (i != choice) {
                    terrains[i].gameObject.SetActive(false);
                }
            }
        }
    }
}