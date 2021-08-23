using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;

namespace APIs  {
    public static class DatabaseAPI {
        private static DatabaseReference _reference;
        private static FirebaseAuth _auth;
        
        public static FirebaseUser user;
        
        public static FirebaseAuth getAuth() {
            if (_auth != null) {
                return _auth;
            }
            _auth = FirebaseAuth.DefaultInstance;
            return _auth;
        }
        
        public static DatabaseReference getDatabase() {
            if (_reference != null) {
                return _reference;
            }

            _reference = FirebaseDatabase.DefaultInstance.RootReference;
            return _reference;
        }

        public static void setAsyncData(string path, object value) {
            DatabaseReference customReference = getReferenceFromPath(path);
            customReference.SetValueAsync(value);
        }
        
        public static Task<DataSnapshot> getAsyncData(string path) {
            DatabaseReference customReference = getReferenceFromPath(path);
            Task<DataSnapshot> task = customReference.GetValueAsync();
            return task;
        }

        private static DatabaseReference getReferenceFromPath(string path) {
            var splitPath = path.Split('/');
            return splitPath.Aggregate(getDatabase(), (current, child) => current.Child(child));
        }
    }
}