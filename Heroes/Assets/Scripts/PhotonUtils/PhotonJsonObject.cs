using Newtonsoft.Json;

namespace PhotonUtils
{
    public class PhotonJsonObject
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
