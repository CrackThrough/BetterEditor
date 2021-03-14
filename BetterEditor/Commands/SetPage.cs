using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using UnityEngine;

namespace BetterEditor.Commands
{
    [CommandInfo(Id = "set_page")]
    class SetPage : BECommand
    {
        public SetPage() : base()
        {

        }

        public override void Execute(scnEditor instance, string[] args)
        {
            object myObj = castStringToType(args[0]);

            if (isType(myObj, out int page))
            {
                int currentPage;
                int maxPage;

                object intObj = scnEditorPrivates.GetField("currentPage");
                currentPage = (int)(intObj ?? 0);

                intObj = scnEditorPrivates.GetField("maxPage");
                maxPage = (int) (intObj ?? 1);

                currentPage = Mathf.Clamp(currentPage + page, 0, maxPage);
                scnEditorPrivates.SetField("currentPage", currentPage);
                instance.ShowEventsPage(currentPage);
            }
        }
    }
}
