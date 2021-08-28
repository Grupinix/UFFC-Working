using System.Collections.Generic;
using Default;
using UnityEngine;
using UnityEngine.UI;
using UserData;

namespace Room {
    public class UserInterface : MonoBehaviour {

        public bool canDropMana;
        
        private int _redManaGen;
        private int _redManaInPool;
        private int _blueManaGen;
        private int _blueManaInPool;
        private int _greenManaGen;
        private int _greenManaInPool;
            
        private int _actualCardId;

        [SerializeField] private int life = 20;
        [SerializeField] private List<Material> materials;
        [SerializeField] private GameObject cardInfoPanel;
        [SerializeField] private Image cardImage;
        [SerializeField] private Slider barraDeVida;
        [SerializeField] private Text textoVida;
        [SerializeField] private Text redMana;
        [SerializeField] private Text blueMana;
        [SerializeField] private Text greenMana;

        [SerializeField] private GameObject deck;

        private DeckController _deckController;

        private void Start() {
            _deckController = deck.GetComponent<DeckController>();
        }

        public void openCardView(int cardId) {
            _actualCardId = cardId;
            cardInfoPanel.SetActive(true);
            cardImage.material = materials[cardId];
        }

        public void castCard() {
            if (!canDropMana) {
                return;
            }

            canDropMana = false;
            CardStatus cardStatus = UserDeck.getCardStatus(_actualCardId);
            if (cardStatus.isTerrain) {
                if (cardStatus.manaGen[0] > 0) {
                    _redManaGen += cardStatus.manaGen[0];
                    _redManaInPool += cardStatus.manaGen[0];
                    redMana.text = _redManaInPool.ToString();
                }
                if (cardStatus.manaGen[1] > 0) {
                    _blueManaGen += cardStatus.manaGen[1];
                    _blueManaInPool += cardStatus.manaGen[1];
                    blueMana.text = _blueManaInPool.ToString();
                }
                if (cardStatus.manaGen[2] > 0) {
                    _greenManaGen += cardStatus.manaGen[2];
                    _greenManaInPool += cardStatus.manaGen[2];
                    greenMana.text = _greenManaInPool.ToString();
                }
            }
            _deckController.removerCarta(_actualCardId);
            cardInfoPanel.SetActive(false);
        }

        public void resetMana() {
            _redManaInPool = _redManaGen;
            redMana.text = _redManaInPool.ToString();
            _blueManaInPool = _blueManaGen;
            blueMana.text = _blueManaInPool.ToString();
            _greenManaInPool = _greenManaGen;
            greenMana.text = _greenManaInPool.ToString();
        }

        public void removerVida(int amount) {
            life -= amount;
            textoVida.text = life.ToString();
            barraDeVida.value = life;
        }

        public void closeGame() {
            Application.Quit();
        }

        public void closePanel() {
            cardInfoPanel.SetActive(false);
        }
        
    }
}