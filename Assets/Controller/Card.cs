using UnityEngine;
using UnityEngine.UI;

namespace Quest.Core.View{
public class Card {
  public string name;
  public string image;

	public Card (string name, string image)
	{
    this.name = name;
    this.image = image;
	}
}
}
