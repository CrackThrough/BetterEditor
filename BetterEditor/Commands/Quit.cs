using BetterEditor.Core;
using BetterEditor.Core.Attributes;

namespace BetterEditor.Commands
{
	[CommandInfo(Id = "quit")]
	public class Quit : BECommand
	{
		public override void Execute(scnEditor instance, string[] args)
		{
			var force = false;
			if (args.Length > 0)
				bool.TryParse(args[0], out force);

			scnEditorPrivates.InvokeMethod(force ? "QuitToMenu" : "TryQuitToMenu");
		}
	}
}