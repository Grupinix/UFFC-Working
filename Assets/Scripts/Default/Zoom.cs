using UnityEngine;

namespace Default {
    
    /**
     * Classe responsável pelo sistema
     * de zoom in e zoom out da cena
     * de montagem do baralho
     */
    public class Zoom : MonoBehaviour {

        [SerializeField] private float zoomOutMin = 1;
        [SerializeField] private float zoomOutMax = 8;
        [SerializeField] private Transform zoomTransform;

        private void Update() {

            if (Input.touchCount == 2) {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * -0.01f);
            }

            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        private void zoom(float increment) {
            Vector3 vector3 = zoomTransform.localScale;
            
            vector3.x = Mathf.Clamp(vector3.x - increment, zoomOutMin, zoomOutMax);
            vector3.y = Mathf.Clamp(vector3.y - increment, zoomOutMin, zoomOutMax);
            
            zoomTransform.localScale = vector3;
        }
    }
}