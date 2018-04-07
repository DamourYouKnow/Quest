using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quest.Utils {
    public abstract class SerializableObject {
        private MarkupConverter converter;

        public SerializableObject() {
            this.converter = new MarkupConverter();
        }

        public MarkupConverter Converter {
            get { return this.converter; }
        }
    }


    public class MarkupConverter {
        private JsonConversion jsonConversion;

        public JsonConversion Json {
            get {
                if (this.jsonConversion == null) return new DefaultJsonConversion();
                return this.jsonConversion;
            }
            set { this.jsonConversion = value; }
        }
    }


    public abstract class MarkupConversion {
        public abstract string ToString(SerializableObject obj);
    }


    public abstract class JsonConversion : MarkupConversion {
        public override string ToString(SerializableObject obj) {
            return this.ToJObject(obj).ToString();
        }

        public abstract JObject ToJObject(SerializableObject obj);
    }


    public class DefaultJsonConversion : JsonConversion {
        public override JObject ToJObject(SerializableObject obj) {
            return JObject.Parse(JsonConvert.SerializeObject(obj));
        }
    }


    public class Random {
        public static void Shuffle<T>(IList<T> list) {
            System.Random rand = new System.Random();
            for (int i = 0; i < list.Count; i++) {
                int next = rand.Next(i, list.Count);
                T temp = list[next];
                list[next] = list[i];
                list[i] = temp;
            }
        }
    }


    public class Stringify {
        public static string CommaList<T>(List<T> list) {
            List<string> stringList = new List<string>();
            foreach (T obj in list) {
                stringList.Add(obj.ToString());
            }
            return string.Join(", ", stringList.ToArray());
        }
    }


    public class Jsonify {
        public static List<T> ArrayToList<T>(JToken array) {
            List<T> retList = new List<T>();

            foreach (JToken arrayToken in array as JArray) {
                retList.Add((T)(object)arrayToken);
            }

            return retList;
        }

        public static JArray ListToArray<T>(List<T> list) {
            JArray a = new JArray();
            list.ForEach((x) => a.Add(x));
            return a;
        }
    }
}
