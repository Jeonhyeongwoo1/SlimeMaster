using SlimeMaster.Firebase;
using SlimeMaster.InGame.Manager;

namespace SlimeMaster.Server
{
    public abstract class ServerRequestHandler
    {
        protected FirebaseController _firebaseController;
        protected DataManager _dataManager;

        protected ServerRequestHandler(FirebaseController firebaseController, DataManager dataManager)
        {
            _firebaseController = firebaseController;
            _dataManager = dataManager;
        }
    }
}