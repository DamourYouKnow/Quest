namespace Quest.Core.View{
public class Player {
  public string username;
  public int card_count;
  public int shields;
  public string rank_image;

	public Player (string username, int card_count, int shields, string rank_image)
	{
    this.username = username;
    this.card_count = card_count;
    this.shields = shields;
    this.rank_image = rank_image;
	}
}
}
