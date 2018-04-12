using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Quest.Core.View{
public class Player {
  public string username;
  public int card_count;
  public int shields;
  public string rank_image;
  public List<Card> in_play;


  public Text username_text;
  public Text card_count_text;
  public Text shields_text;
  public Text in_play_text;
  public Image rank_image_image;

	public Player (string username, int card_count, int shields, string rank_image, List<Card> in_play)
	{
    if(username == null || username == ""){
      this.username = "Default";
    }
    this.username = username;
    this.card_count = card_count;
    this.shields = shields;
    this.rank_image = rank_image;
    this.in_play = in_play;
	}
  public void copyValues(Player that){
    this.username = that.username;
    this.card_count = that.card_count;
    this.shields = that.shields;
    this.rank_image = that.rank_image;
    this.in_play = new List<Card>(in_play);
  }

  public void SetGameObjects(GameObject parent){
    this.username_text = parent.GetComponent<Transform>().Find("StatsPanel/OpponentPlayerName").GetComponent<Text>();
    this.card_count_text = parent.GetComponent<Transform>().Find("StatsPanel/Cards/OpponentCardsValue").GetComponent<Text>();
    this.shields_text = parent.GetComponent<Transform>().Find("StatsPanel/Shields/OpponentShieldValue").GetComponent<Text>();
    this.in_play_text = parent.GetComponent<Transform>().Find("StatsPanel/InPlay/OpponentInPlayValue").GetComponent<Text>();
    this.rank_image_image = parent.GetComponent<Transform>().Find("OpponentRankImage").GetComponent<Image>();
  }

  public void CopyGameObjects(Player that){
    this.username_text = that.username_text;
    this.card_count_text = that.card_count_text;
    this.shields_text = that.shields_text;
    this.in_play_text = that.in_play_text;
    this.rank_image_image = that.rank_image_image;
  }

  public void UpdateGameObjects(){
    this.username_text.text = this.username;
    this.card_count_text.text = this.card_count.ToString();
    this.shields_text.text = this.shields.ToString();
    this.in_play_text.text = this.in_play.Count.ToString();
    this.rank_image_image.sprite = (Sprite)Resources.Load<Sprite>(Constants.RESOURCES_CARDS + this.rank_image);
  }
}
}
