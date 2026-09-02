using Packages.CoreUtils.Utils;
using UI.Windows;
using UnityEngine;

namespace UI
{
    public class UnitMenuWindow : Window
    {
        private void Update()
        {
            if (_opened && Input.GetMouseButtonDown(0) && !ObjectButton.IsPointerOverUiElement()) 
            {
                Close();
            }
        }
    }
}
