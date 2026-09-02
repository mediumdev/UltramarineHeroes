using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace UI
{
    public class Settings : MonoBehaviour
    {
        private void OnEnable()
        {
            if (!SaveManagerSafe.GetValue(SavedDataManager.FirstBattleEndedKey, false))
                gameObject.SetActive(false);
        }
    }
}
