using System;

namespace Eclipse.Engine.Core
{
    public abstract class Singleton<T> where T : class
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception($"{typeof(T)} instance not initialized");
                return _instance;
            }
        }

        //public static void Initialize(T instance)
        //{
        //    if (_instance != null)
        //        throw new Exception($"{typeof(T)} instance already initialized");
        //    _instance = instance;
        //}

        public static void CreateInstance()
        {
            if (_instance != null)
                throw new Exception($"{typeof(T)} instance already created");
            _instance = Activator.CreateInstance<T>();
            //_instance = new T();
        }

        public static void ClearInstance()
        {
            _instance = null;
        }
    }

}
