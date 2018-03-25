using Newtonsoft.Json.Linq;

using Quest.Core.Cards;

namespace Quest.Utils {
    public class CardJsonConversion : JsonConversion {
        public override JObject ToJObject(QuestObject obj) {
            Card card = (Card)obj;

            JObject jObj = new JObject();
            jObj["name"] = card.Name;
            jObj["image"] = card.ImageFilename;

            return jObj;
        }
    }
}
