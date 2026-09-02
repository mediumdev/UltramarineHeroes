using UnityEngine;

namespace Pool
{
    public class TransformPool<T, TA> : GenericPool<T, TA> 
        where T : MonoBehaviour, IPoolObject
        where TA : TransformPool<T, TA>
    {
        protected override void Init()
        {
            base.Init();
            gameObject.SetActive(false);
        }

        protected override void InitObj(T obj)
        {
            obj.transform.SetParent(transform);
        }

        public override T Pop(T origin)
        {
            var obj = PopInternal(origin);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public virtual T Pop(T origin, Transform parent, bool show = true)
        {
            var obj = PopInternal(origin);
            obj.transform.SetParent(parent);
            obj.gameObject.SetActive(show);
            return obj;
        }

        public override void Push(T obj)
        {
            var pushedObj = PushInternal(obj);
            pushedObj.transform.SetParent(transform);
        }
    }
}
