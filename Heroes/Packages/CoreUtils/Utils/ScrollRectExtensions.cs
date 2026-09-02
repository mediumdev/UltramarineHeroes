using UnityEngine.UI;

namespace Packages.CoreUtils.Utils
{
    public static class ScrollRectExtensions
    {
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.verticalNormalizedPosition = 1;
        }

        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.verticalNormalizedPosition = 0;
        }
    }
}