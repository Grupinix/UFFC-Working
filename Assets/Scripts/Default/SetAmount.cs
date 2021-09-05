using UnityEngine;
using UnityEngine.UI;
using UserData;

namespace Default {
    
    /**
     * Classe responsável por permitir
     * que o usuário defina uma quantia
     * expecifica de cartas de certo
     * id em seu baralho
     */
    public class SetAmount : MonoBehaviour {

        public int cardId;

        [SerializeField] private Text text;

        /** "Ação" para atualizar o texto de status */
        public void setStatus() {
            text.text = UserDeck.DeckCardIds[cardId].ToString();
        }

        /** "Ação" para incrementar a quantidade dessa carta */
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

        /** "Ação" para decrementar a quantidade dessa carta */
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