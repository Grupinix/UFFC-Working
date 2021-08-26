using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace JsonClasses {

    public class FieldProperties : MonoBehaviour, ISerializable {

        [SerializeField] private List<Transform> cardsPositions;
        [SerializeField] private List<Transform> terrainsPositions;

        [SerializeField] private List<CardProperties> cards;
        [SerializeField] private List<CardProperties> terrains;

        public FieldProperties(SerializationInfo info, StreamingContext streamingContext) {
            cardsPositions = (List<Transform>) info.GetValue("CardsPositions", typeof(List<Transform>));
            terrainsPositions = (List<Transform>) info.GetValue("TerrainsPositions", typeof(List<Transform>));
            cards = (List<CardProperties>) info.GetValue("Cards", typeof(List<CardProperties>));
            terrains = (List<CardProperties>) info.GetValue("Terrains", typeof(List<CardProperties>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext streamingContext) {
            info.AddValue("CardsPositions", cardsPositions);
            info.AddValue("TerrainsPositions", terrainsPositions);
            info.AddValue("Cards", cards);
            info.AddValue("Terrains", terrains);
        }
    }
}