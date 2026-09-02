using CoreConfigs.Configs;
using Newtonsoft.Json;
using UnityEngine;

namespace Configs
{
    public class UiConfig : ConfigBase
    {
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Sprite _descriptionIcon;

        [JsonIgnore]
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [JsonIgnore] public string Description => _description;
        [JsonIgnore] public Sprite Icon => _icon;
        [JsonIgnore] public Sprite DescriptionIcon => _descriptionIcon;
    }
}