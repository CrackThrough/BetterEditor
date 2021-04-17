using BetterEditor.Core;
using BetterEditor.Core.Attributes;

namespace BetterEditor.Commands
{
	[CommandInfo(Id = "upload")]
	public class Upload : BECommand
	{
		public override void Execute(scnEditor instance, string[] args)
		{
			instance.publishWindow.windowContainer.SetActive(true);
			instance.publishWindow.Init();
			scnEditorPrivates.InvokeMethod("ShowEventPicker", new object[] { false });
			instance.settingsPanel.ShowInspector(false);
			instance.levelEventsPanel.ShowInspector(false);
		}
	}
}
