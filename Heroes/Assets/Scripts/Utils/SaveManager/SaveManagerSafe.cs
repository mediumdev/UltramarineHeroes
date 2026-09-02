using Newtonsoft.Json.Linq;

namespace Utils.SaveManager
{
    public static class SaveManagerSafe
    {
        private const int Salt = 778514367;

        public static void Add(string key, JToken value)
        {
            var hashedKey = key.GetHashCode();
            var hashedValue = JToken.FromObject(value).GetHashCode();
            var hashedSaltedValue = (hashedValue ^ Salt).GetHashCode();

            Utils.SaveManager.SaveManager.Add(hashedKey.ToString(), value);
            Utils.SaveManager.SaveManager.Add(("_" + key).GetHashCode().ToString(),
                hashedSaltedValue);
        }


        public static T GetValue<T>(string key, T wrongResult = default)
        {
            var hashedKey = key.GetHashCode();
            var loadedHash =
                Utils.SaveManager.SaveManager.GetValue<int>(("_" + key).GetHashCode().ToString());
            var clearValue = Utils.SaveManager.SaveManager.GetValue<T>(hashedKey.ToString());
            var clearValueJToken =
                Utils.SaveManager.SaveManager.GetValue<JToken>(hashedKey.ToString());
            var hashedClearValue = clearValueJToken?.GetHashCode() ?? clearValue.GetHashCode();


            if (loadedHash != (hashedClearValue ^ Salt).GetHashCode())
            {
                return wrongResult;
            }

            T defaultValue = default;
            return Utils.SaveManager.SaveManager.GetValue(hashedKey.ToString(), defaultValue);
        }

        public static void Remove(string key)
        {
            var hashedKey = key.GetHashCode();
            var hashedDuplicateKey = ("_" + key).GetHashCode();
            if (HasKey(key) && Utils.SaveManager.SaveManager.HasKey(hashedDuplicateKey.ToString()))
            {
                Utils.SaveManager.SaveManager.Remove(hashedKey.ToString());
                Utils.SaveManager.SaveManager.Remove(hashedDuplicateKey.ToString());
            }
        }

        public static bool HasKey(string key)
        {
            return Utils.SaveManager.SaveManager.HasKey(key.GetHashCode().ToString());
        }
    }
}