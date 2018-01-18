using System.Collections.Generic;

namespace Utils {
    public class Random {
        public static void Shuffle<T>(IList<T> list) {
            System.Random rand = new System.Random();
            for (int i = 0; i < list.Count; i++) {
                int next = rand.Next(i, list.Count - 1);
                T temp = list[next];
                list[next] = list[i];
                list[i] = temp;
            }
        }
    }
}