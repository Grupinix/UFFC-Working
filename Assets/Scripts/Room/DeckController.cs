using System.Collections.Generic;
using JsonClasses;
using UnityEngine;
using UserData;
using Random = UnityEngine.Random;

namespace Room {
    public class DeckController : MonoBehaviour {
        private Vector3 _deckScale;
        private Vector3 _positionHand;
        private Vector3 _targetPosition;

        private int _totalInitialCards;
        private List<int> _deckListCards;

        [SerializeField] private GameObject manager;
        [SerializeField] private GameObject enemyDeck;
        
        [SerializeField] private Vector3 rangeCardPosition;
        [SerializeField] private Vector3 positionShowPlayer;
        
        [SerializeField] private GameObject camObj;
        [SerializeField] private GameObject hand;
        [SerializeField] private GameObject cardPrefab;
        
        // animation variables
        [SerializeField] private float timeToShowPlayer;
        [SerializeField] private float dumbGetCard;

        private UserInterface _userInterface;
        private float _currentTimeToShowPlayer;
        private Camera _camera;

        private GameObject _tempCard;


        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private Vector3 _positionNextCard;

        private List<CardProperties> cards = new List<CardProperties>();

        public static bool canBuy;

        private void Start() {
            _camera = camObj.GetComponent<Camera>();
            _userInterface = manager.GetComponent<UserInterface>();
            _deckListCards = UserDeck.getDeckCards();
            _deckScale = transform.localScale;
            _totalInitialCards = _deckListCards.Count;

            Vector3 handPosition = hand.transform.position;
            _minPosition = handPosition - rangeCardPosition;
            _maxPosition = handPosition + rangeCardPosition;
        }

        private void Update() {
            if (_tempCard != null) {
                _currentTimeToShowPlayer += Time.deltaTime;
                if (_currentTimeToShowPlayer > timeToShowPlayer) {
                    _positionHand = _positionNextCard;
                    _targetPosition = _positionHand;
                }
                
                _tempCard.transform.position = Vector3.Lerp(_tempCard.transform.position, _targetPosition + new Vector3(0, 0.2f, 0), dumbGetCard * Time.deltaTime);
            }

            if (Input.touchCount == 1) {
                checkHit(_camera.ScreenPointToRay(Input.touches[0].position), false);
            }
            else if (Input.GetMouseButtonDown(0)) {
                checkHit(_camera.ScreenPointToRay(Input.mousePosition), true);
            }
        }

        private void checkHit(Ray ray, bool pc) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (canBuy && hit.collider.gameObject.name == "Deck") {
                    if (pc) {
                        getCard();
                        return;
                    }

                    Touch screenTouch = Input.GetTouch(0);
                    if (screenTouch.phase == TouchPhase.Ended) {
                        getCard();
                    }
                }
                else if (hit.collider.gameObject.name == "Front") {
                    if (pc) {
                        _userInterface.openCardView(hit.collider.gameObject.GetComponentInParent<CardProperties>().cardId);
                        return;
                    }

                    Touch screenTouch = Input.GetTouch(0);
                    if (screenTouch.phase == TouchPhase.Ended) {
                        _userInterface.openCardView(hit.collider.gameObject.GetComponentInParent<CardProperties>().cardId);
                    }
                }
            }
        }

        private void getCard() {
            if (_deckListCards.Count > 0 && cards.Count < 5) {
                int randCardIndex = Random.Range(0, _deckListCards.Count);
                int id = _deckListCards[randCardIndex];
                _deckListCards.RemoveAt(randCardIndex);

                _tempCard = Instantiate(cardPrefab, hand.transform);
                _tempCard.transform.rotation = Quaternion.Euler(15, 180, 0);
                CardProperties cardProperties = _tempCard.GetComponent<CardProperties>();
                cardProperties.cardId = id;
                cardProperties.setMaterial();

                resizeDeck();

                _targetPosition = positionShowPlayer;
                _currentTimeToShowPlayer = 0;
                addCard(_tempCard.GetComponent<CardProperties>());
                canBuy = false;
            }
        }

        private void resizeDeck() {
            resizeBoxSize(transform);
            resizeBoxSize(enemyDeck.transform);
        }

        private void resizeBoxSize(Transform objTransform) {
            Vector3 newSize = objTransform.localScale;

            newSize.y = _deckListCards.Count * _deckScale.y / _totalInitialCards;
            objTransform.localScale = newSize;

            if (_deckListCards.Count == 0) {
                objTransform.GetComponent<Renderer>().enabled = false;
            }
        }

        public void removerCarta(int cardId) {
            int remove = 9999;
            for (int i = 0; i < cards.Count; i++) {
                if (cards[i].cardId == cardId) {
                    remove = i;
                }
            }

            if (remove == 9999) {
                return;
            }

            CardProperties cardProperties = cards[remove];
            cards.RemoveAt(remove);
            if (_tempCard.Equals(cardProperties.gameObject)) {
                _tempCard = null;
            }
            Destroy(cardProperties.gameObject);
        }
        
        private void reorganizeCards(){
            for(int i = 1; i < cards.Count; i++){
                Vector3 position = calcDistanceHandPosition(i, cards.Count + 1);
                if(i-1 < cards.Count){
                    cards[i-1].transform.position = position;
                }
            }
        
            _positionNextCard = calcDistanceHandPosition(cards.Count, cards.Count + 1);
        }

        private Vector3 calcDistanceHandPosition(int indice, int limit) {
            float distance = indice / (float) limit;

            return Vector3.Lerp(_minPosition, _maxPosition, distance);
        }

        private void addCard(CardProperties card){
            cards.Add(card);
            reorganizeCards();
        }
    }
}