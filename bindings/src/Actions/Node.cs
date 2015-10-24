﻿using System;
using System.Threading.Tasks;

namespace Urho
{
	partial class Node
	{
		/// <summary>
		/// Runs a sequence of Actions so that it can be awaited.
		/// </summary>
		/// <param name="actions">An array of FiniteTimeAction objects.</param>
		public Task<ActionState> RunActionsAsync(params FiniteTimeAction[] actions)
		{
			if (actions.Length == 0)
				return Task.FromResult<ActionState>(null);

			var tcs = new TaskCompletionSource<ActionState>();

			var numActions = actions.Length;
			var asyncActions = new FiniteTimeAction[actions.Length + 1];
			Array.Copy(actions, asyncActions, numActions);

			ActionState state = null;
			asyncActions[numActions] = new CallFunc(() => tcs.TrySetResult(state));

			var asyncAction = asyncActions.Length > 1 ? new Sequence(asyncActions) : asyncActions[0];

			state = Application.Current.ActionManager.AddAction(asyncAction, this, !IsRunning);
			return tcs.Task;
		}

		public void RemoveAllActions()
		{
			Application.Current.ActionManager.RemoveAllActionsFromTarget(this);
		}

		public bool IsRunning => true;
// TODO: handle platform's Pause, Resume etc..
	}
}
