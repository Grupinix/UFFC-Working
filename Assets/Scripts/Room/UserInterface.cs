using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Room {
    public class UserInterface : MonoBehaviour {

        [SerializeField] private List<Material> materials;
        [SerializeField] private GameObject cardInfoPanel;
        [SerializeField] private Image cardImage;

        public void openCardView(int cardId) {
            cardInfoPanel.SetActive(true);
            cardImage.material = materials[cardId];
        }

        public void closePanel() {
            cardInfoPanel.SetActive(false);
        }
        
    }
}