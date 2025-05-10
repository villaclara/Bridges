using System;

public interface IGameStage
{
	void ExecuteStage();

	event Action OnStageExecutionCompleted;
}
