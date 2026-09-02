using Photon.Pun;
using UnityEngine;

namespace PhotonUtils
{
    public abstract class MonoPunSingleton<T> : MonoBehaviourPunCallbacks where T : MonoPunSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gobj = new GameObject(typeof(T).ToString());
                    _instance = gobj.AddComponent<T>();
                    _instance.Init();
                }
                return _instance;
            }
        }

        protected virtual void Init()
        {
        }
    }
}
