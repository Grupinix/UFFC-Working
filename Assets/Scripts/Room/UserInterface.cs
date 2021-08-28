using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Room {
    public class UserInterface : MonoBehaviour {
        
        private int _actualCardId;

        [SerializeField] private int life = 20;
        [SerializeField] private List<Material> materials;
        [SerializeField] private GameObject cardInfoPanel;
        [SerializeField] private Image cardImage;
        [SerializeField] private Slider barraDeVida;
        [SerializeField] private Text textoVida;

        public void openCardView(int cardId) {
            _actualCardId = cardId;
            cardInfoPanel.SetActive(true);
            cardImage.material = materials[cardId];
        }

        public void castCard() {
            
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