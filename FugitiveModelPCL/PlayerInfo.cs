using Newtonsoft.Json;
using System.Collections.Generic;

namespace FugitiveModel
{
    public class PlayerInfo
    {
        public Role Role { get; set; }
        public List<HideoutCard> Hand { get; set; }
        public HashSet<int> GetHandIds()
        {
            HashSet<int> ids = new HashSet<int>();
            foreach(HideoutCard card in Hand)
            {
                ids.Add(card.Hideout);
            }
            return ids;
        }

        public static PlayerInfo fromJson(string json)
        {
            return JsonConvert.DeserializeObject<PlayerInfo>(json);
        }
    }
}
