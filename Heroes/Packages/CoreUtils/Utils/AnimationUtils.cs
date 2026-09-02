using System;
using System.Collections;
using UnityEngine;

namespace Packages.CoreUtils.Utils
{
    public static class AnimationUtils
    {
        public static void WaitUntilComplete(this Animation animation, MonoBehaviour parent, Action callback)
        {
            parent.StartCoroutine(WaitComplete(animation, callback));
        }

        private static IEnumerator WaitComplete(Animation animation, Action callback)
        {
            yield return new WaitUntil(() => !animation.isPlaying);
            callback?.Invoke();
        }
    }
}