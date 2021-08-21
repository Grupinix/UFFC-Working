using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace APIs {
    public class FirebaseInit : MonoBehaviour {

        public UnityEvent OnFirebaseInitialized = new UnityEvent();
        
        private void Start() {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (DependencyStatus.Available != task.Result) {
                    return;
                }
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                OnFirebaseInitialized.Invoke();
            });
        }
    }
}