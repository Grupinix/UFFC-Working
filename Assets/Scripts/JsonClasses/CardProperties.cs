using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace JsonClasses {

    [Serializable]
    public class CardProperties : MonoBehaviour, ISerializable {

        public int cardId;

        [SerializeField] private List<Material> materials;
        
        [SerializeField] private int cardPower;
        [SerializeField] private int cardDefense;
        
        [SerializeField] private GameObject carta;

        public void setMaterial(){
            Renderer img = carta.GetComponent<Renderer>();
            img.material = materials[cardId];
        }
        
        public CardProperties(SerializationInfo info, StreamingContext streamingContext) {
            cardId = (int) info.GetValue("CardId", typeof(int));
            cardPower = (int) info.GetValue("CardPower", typeof(int));
            cardDefense = (int) info.GetValue("CardDefense", typeof(int));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext streamingContext) {
            info.AddValue("CardId", cardId);
            info.AddValue("CardPower", cardPower);
            info.AddValue("CardDefense", cardDefense);
        }
    }
}