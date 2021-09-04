using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Room {
    
    // Escolhe um terreno aleatório
    
    public class RandomTerrain : MonoBehaviour {
        [SerializeField] private List<Terrain> terrains;

        private void Start() {
            Random random = new Random(DateTime.Now.Millisecond);
            terrains[random.Next(terrains.Count)].gameObject.layer = LayerMask.NameToLayer("TerrainActive");
        }
    }
}