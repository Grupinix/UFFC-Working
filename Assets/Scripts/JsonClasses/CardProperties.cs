using System;
using System.Collections.Generic;
using UnityEngine;

namespace JsonClasses {

    [Serializable]
    public class CardProperties : MonoBehaviour {

        public int cardId;

        [SerializeField] private List<Material> materials;
        
        [SerializeField] private int cardPower;
        [SerializeField] private int cardDefense;
        
        [SerializeField] private GameObject carta;

        public void setMaterial(){
            Renderer img = carta.GetComponent<Renderer>();
            img.material = materials[cardId];
        }
    }
}