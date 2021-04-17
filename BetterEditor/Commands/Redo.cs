using System.Linq;
using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using UnityEngine;

namespace BetterEditor.Commands
{
	[CommandInfo(Id = "redo")]
	public class Redo : BECommand
	{
		public override void Execute(scnEditor instance, string[] args)
		{
			if (instance.changingState != 0 || instance.changingFile)
				return;

			var count = 1;
			if (args.Length > 0)
				int.TryParse(args[0], out count);

			var source = instance.redoStates;
			switch (count = Mathf.Clamp(count, 0, source.Count))
			{
				case 0:
					return;
				case 1:
					instance.Redo();
					break;
			}

			instance.SaveState(false);
			for (var i = 0; i < count - 1; i++)
				instance.undoStates.Add(instance.redoStates.Pop());

			++instance.changingState;
			var levelState = source.Last();
			var selectedFloors = levelState.selectedFloors;

			if (levelState.data != null)
			{
				instance.customLevel.levelData = levelState.data;
				instance.printe(levelState.data.pathData);
				scnEditorPrivates.InvokeMethod("DeselectFloors");
				instance.RemakePath();
			}

			if (selectedFloors.Count > 1)
				scnEditorPrivates.InvokeMethod("MultiSelectFloors",
					new object[] { instance.customLevel.levelMaker.listFloors[selectedFloors[0]], instance.customLevel.levelMaker.listFloors[selectedFloors[selectedFloors.Count - 1]] });
			else if (selectedFloors.Count == 1)
			{
				var num = selectedFloors[0];
				scnEditorPrivates.InvokeMethod("SelectFloor", new object[] { instance.customLevel.levelMaker.listFloors[num] });
				instance.levelEventsPanel.ShowPanel(levelState.floorEventType, num);
			}

			instance.settingsPanel.ShowPanel(levelState.settingsEventType);
			source.Remove(levelState);
			--instance.changingState;
		}
	}
}
