using UI.Windows;

namespace UI.UIWindows.Campaign
{
    public class CampaignWindow : Window
    {
        public void BackToLobby()
        {
            GoToScene.LoadScene("Lobby");
        }
    }
}
