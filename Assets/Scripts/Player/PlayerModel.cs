public class PlayerModel : IPlayerModel
{
	public int Id { get; } = 0;
	public string ColorHEX { get; set; } = "#000000";
	public int BridgesCount { get; set; } = 0;
	public bool IsMyTurn { get; set; }

	public PlayerModel(int id, string colorHex)
	{
		Id = id;
		ColorHEX = colorHex;
		BridgesCount = 0;
		IsMyTurn = false;
	}
}

public interface IPlayerModel
{
	int Id { get; }
	string ColorHEX { get; set; }
	int BridgesCount { get; set; }
	bool IsMyTurn { get; set; }
}

public enum PlayerTurn
{
	P1_Turn = 1,
	P2_Turn = 2,
}