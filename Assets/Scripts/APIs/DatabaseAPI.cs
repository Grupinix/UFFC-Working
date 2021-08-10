using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;

namespace APIs  {
    public static class DatabaseAPI {
        private static DatabaseReference _reference;

        private static DatabaseReference getDatabase() {
            if (_reference != null) {
                return _reference;
            }
            return _reference = FirebaseDatabase.DefaultInstance.RootReference;
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