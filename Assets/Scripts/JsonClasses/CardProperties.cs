using System;
using System.Collections.Generic;
using Default;
using UnityEngine;
using UserData;

namespace JsonClasses {

    /**
     * Classe serializavel responsável
     * por armazenar as propriedades
     * de uma carta
     */
    [Serializable]
    public class CardProperties : MonoBehaviour {

        public int cardId;

        [SerializeField] private List<Material> materials;

        [SerializeField] private GameObject carta;
        [SerializeField] private Material verse;
        
        public int cardPower;
        public int cardDefense;

        public bool ataque;

        /** "ação" para definir a textura da carta */
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

        /** "ação" para definir o poder padrão da carta */
        public void resetPower() {
            CardStatus cardStatus = UserDeck.getCardStatus(cardId);
            cardPower = cardStatus.cardPower;
            cardDefense = cardStatus.cardDefense;
        }
    }
}