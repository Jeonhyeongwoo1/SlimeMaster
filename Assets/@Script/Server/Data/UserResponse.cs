using System;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class UserResponse : Response
    {
        public DBUserData DBUserData;
        public DateTime LastLoginTime;
    }
}