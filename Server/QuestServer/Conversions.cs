using Newtonsoft.Json.Linq;

using Quest.Core.Cards;
using Quest.Core.Players;

namespace Quest.Utils {
    public class CardJsonConversion : JsonConversion {
        public override JObject ToJObject(SerializableObject obj) {
            Card card = obj as Card;

            JObject jObj = new JObject();
            jObj["name"] = card.Name;
            jObj["image"] = card.ImageFilename;

            return jObj;
        }
    }


    public class PlayerJsonConversion : JsonConversion {
        public override JObject ToJObject(SerializableObject obj) {
            Player player = obj as Player;

            JObject jObj = new JObject();
            jObj["username"] = player.Username;
            jObj["card_count"] = player.Hand.Count;
            jObj["shields"] = player.Rank.Shields;
            jObj["rank_image"] = player.RankCard.ImageFilename;

            return jObj;
        }
    }
}
