using Default;
using JsonClasses;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserData;

namespace Lobby {
    public class DeckChange : MonoBehaviour {

        [SerializeField] private GameObject parent;
        [SerializeField] private GameObject element;

        public void loadLobbyScene() {
            SceneManager.LoadScene("Lobby");
        }

        private void Start() {
            for (int i = 0; i < UserDeck.getAmountOfCards(); i++) {
                GameObject card = Instantiate(element, parent.transform);
                
                SetAmount setAmount = card.GetComponent<SetAmount>();
                setAmount.cardId = i;
                setAmount.setStatus();
                
                CardProperties cardProperties = card.GetComponentInChildren<CardProperties>();
                cardProperties.cardId = i;
                cardProperties.setMaterial();
            }
        }
    }
}