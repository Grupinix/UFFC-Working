using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Database;
using UnityEngine;

namespace APIs {
    public class FirebaseInit : MonoBehaviour {
        private void Start() {
            Task asyncTask = FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => { 
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true); 
            });
            setUpDatabase(asyncTask);
        }

        private async void setUpDatabase(Task asyncTask) {
            await Task.WhenAll(asyncTask);

            DatabaseAPI.setDefaultDatabase(FirebaseDatabase.DefaultInstance.RootReference);
        }
    }
}