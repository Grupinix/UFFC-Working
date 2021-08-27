using UnityEngine;
using UnityEngine.UI;
using UserData;

namespace Default {
    public class SetAmount : MonoBehaviour {
        public int cardId;

        [SerializeField] private Text text;

        public void setStatus() {
            text.text = UserDeck.DeckCardIds[cardId].ToString();
        }

        public void onAdd() {
            int newAmount = UserDeck.DeckCardIds[cardId] + 1;
            if (newAmount > UserDeck.getCardStatus(cardId).maxCopies) {
                return;
            }
            
            PlayerPrefs.SetInt($"card_{cardId}", newAmount);
            PlayerPrefs.Save();
            UserDeck.DeckCardIds[cardId] = newAmount;
            text.text = newAmount.ToString();
        }

        public void onRemove() {
            int newAmount = UserDeck.DeckCardIds[cardId] - 1;
            if (newAmount < 0) {
                return;
            }
            
            PlayerPrefs.SetInt($"card_{cardId}", newAmount);
            PlayerPrefs.Save();
            UserDeck.DeckCardIds[cardId] = newAmount;
            text.text = newAmount.ToString();
        }

    }
}