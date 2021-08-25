using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Room {
    public class RandomTerrain : MonoBehaviour {
        [SerializeField] private List<TerrainData> terrains;

        private void Start() {
            Random random = new Random(DateTime.Now.Millisecond);
            TerrainData terrain = terrains[random.Next(terrains.Count)];
            Terrain.CreateTerrainGameObject(terrain);
        }
    }
}