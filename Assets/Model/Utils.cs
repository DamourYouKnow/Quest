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
	//Registers to a Subject to recieve updates
	public abstract class Observer
	{
		abstract public void update ();
	}
	//Notifies registered Observers on update
	public abstract class Subject
	{
		protected HashSet<Observer> observers;

		public void notify(){
			foreach (Observer obs in observers) {
				obs.update();
			}
		}

		public void register (Observer observer){
			observers.Add (observer);
		}

		public void unregister(Observer observer){
			observers.Remove (observer);
		}
	}
}