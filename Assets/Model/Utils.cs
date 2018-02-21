using System.Collections.Generic;
using UnityEngine;

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
	//Registers to a Subject to recieve updates
	public abstract class Observer
	{
		public abstract void updateObserver ();
	}
	//Notifies registered Observers on update
	public abstract class Subject
	{
		protected HashSet<Observer> observers;

		public void notify(){
			foreach (Observer obs in observers) {
				obs.updateObserver();
			}
		}

		public void register (Observer observer){
			observers.Add (observer);
		}

		public void unregister(Observer observer){
			observers.Remove (observer);
		}
	}

    public class UnityFont {
        public static Font Find(string fontName) {
            return Resources.GetBuiltinResource(typeof(Font), fontName + ".ttf") as Font;
        }

        public static Font Arial() {
            return UnityFont.Find("Arial");
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