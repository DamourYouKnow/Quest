﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quest.Utils {
    public abstract class QuestObject {
        private MarkupConverter converter;

        public QuestObject() {
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
        public abstract string ToString(QuestObject obj);
    }


    public abstract class JsonConversion : MarkupConversion {
        public override string ToString(QuestObject obj) {
            return this.ToJObject(obj).ToString();
        }

        public abstract JObject ToJObject(QuestObject obj);
    }


    public class DefaultJsonConversion : JsonConversion {
        public override JObject ToJObject(QuestObject obj) {
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
}
