using System;

/// <summary>
/// Represents each separate game stage.
/// </summary>
public interface IGameStage
{
	/// <summary>
	/// Start executing this stage.
	/// </summary>
	void ExecuteStage();

	/// <summary>
	/// Event is called when the conditions are met to stop executing (stage has come to end).
	/// </summary>
	event Action OnStageExecutionCompleted;

	/// <summary>
	/// Resets all values to default, clears lists etc to restart the game.
	/// </summary>
	void ResetStage();
}
