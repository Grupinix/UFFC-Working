using System;
using System.Collections.Generic;
using Default;
using UnityEngine;
using UserData;

namespace JsonClasses {

    [Serializable]
    public class CardProperties : MonoBehaviour {

        public int cardId;

        [SerializeField] private List<Material> materials;

        [SerializeField] private GameObject carta;
        [SerializeField] private Material verse;
        
        public int cardPower;
        public int cardDefense;

        public void setMaterial(){
            Renderer img = carta.GetComponent<Renderer>();
            
            if (cardId == 9999) {
                img.material = verse;
                cardPower = 0;
                cardDefense = 0;
                return;
            }
            
            img.material = materials[cardId];
            CardStatus cardStatus = UserDeck.getCardStatus(cardId);
            cardPower = cardStatus.cardPower;
            cardDefense = cardStatus.cardDefense;
        }
    }
}