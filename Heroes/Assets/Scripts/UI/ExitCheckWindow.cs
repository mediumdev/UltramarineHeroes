using Network;
using PhotonUtils;
using UI.Windows;

public class ExitCheckWindow : Window
{
    public void RestartBattle()
    {
        Close();
        PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.GameEnded, null);
    }
}
