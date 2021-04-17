using BetterEditor.Core;
using BetterEditor.Core.Attributes;

namespace BetterEditor.Commands
{
	[CommandInfo(Id = "tilenum")]
	public class TileNum : BECommand
	{
		public override void Execute(scnEditor instance, string[] args)
		{
			if (args.Length < 1)
				return;

			var oldVal = scnEditorPrivates.GetField<bool>("showFloorNums");

			switch (args[0].ToLower())
			{
				case "on":
				case "true":
				case "yes":
					if (oldVal)
						return;
					break;
				case "off":
				case "false":
				case "no":
					if (!oldVal)
						return;
					break;
				case "toggle":
				case "swap":
				case "switch":
					break;
				default:
					return;
			}

			var index = instance.selectedFloor != null ? instance.selectedFloor.seqID : -1;
			scnEditorPrivates.SetField("showFloorNums", !oldVal);
			instance.RemakePath();
			if (index != -1)
				scnEditorPrivates.InvokeMethod("SelectFloor", new object[] { instance.customLevel.levelMaker.listFloors[index], false });
		}
	}
}