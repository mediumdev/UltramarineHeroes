using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;

namespace Game.Controllers
{
    public class GameSettings : MonoSingleton<GameSettings>
    {
        private GameSettingsConfig _settings;

        public GameSettingsConfig Settings => _settings;

        protected override void Init()
        {
            base.Init();

            if (_settings == null)
                _settings = ConfigBase.LoadFirstAvailableConfig<GameSettingsConfig>();
            
            DontDestroyOnLoad(gameObject);
        }
        
        public void InitSettings(){}
    }
}