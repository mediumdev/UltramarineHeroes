using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Packages.CoreUtils.Utils
{
    public class ObjectButton : MonoBehaviour
    {
        [SerializeField] private Button.ButtonClickedEvent onClick = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnClick => onClick;

        private static bool IsPointerOverGameObject()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return true;
            }

            return false;
        }

        private void OnMouseDown()
        {
            if (IsPointerOverUiElement())
                return;

            if (IsPointerOverGameObject())
                return;

            onClick?.Invoke();
        }


        public static bool IsPointerOverUiElement()
        {
            return IsPointerOverUiElement(GetEventSystemRayCastResults());
        }

        private static bool IsPointerOverUiElement(List<RaycastResult> eventSystemRayCastResults)
        {
            for (int index = 0; index < eventSystemRayCastResults.Count; index++)
            {
                RaycastResult curRayCastResult = eventSystemRayCastResults[index];
                if (curRayCastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                    return true;
            }

            return false;
        }

        static List<RaycastResult> GetEventSystemRayCastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> rayCastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, rayCastResults);
            return rayCastResults;
        }
    }
}