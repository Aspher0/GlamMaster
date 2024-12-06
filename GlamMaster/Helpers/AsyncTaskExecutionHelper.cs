using System;
using System.Threading.Tasks;

namespace GlamMaster.Helpers;

/*
 * This class is used when you need to be able to wait for a Task to finish before executing some other code.
 * 
 * Useful when "await" crashes the whole game.
 */

public class AsyncTaskExecutionHelper
{
    private Task? _currentTask;
    private Action? _nextAction;

    public bool IsTaskRunning => _currentTask != null && !_currentTask.IsCompleted;

    public void StartTask(Task task, Action? nextAction)
    {
        if (IsTaskRunning) throw new InvalidOperationException("A task is already running.");

        _currentTask = task;
        _nextAction = nextAction;

        MonitorTask();
    }

    private async void MonitorTask()
    {
        try
        {
            if (_currentTask != null)
                await _currentTask;

            _nextAction?.Invoke();
        }
        catch (Exception ex)
        {
            GlamLogger.Error($"Error in task: {ex.Message}");
        }
        finally
        {
            _currentTask = null;
            _nextAction = null;
        }
    }
}
