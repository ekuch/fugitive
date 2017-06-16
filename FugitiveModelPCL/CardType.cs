using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FugitiveModel
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CardType
    {
        HIDEOUT_FOUR_TO_FOURTEEN,
        HIDEOUT_FIFTEEN_TO_TWENTYEIGHT,
        HIDEOUT_TWENTYNINE_TO_FOURTYONE
    }
}
