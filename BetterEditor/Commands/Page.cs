using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using UnityEngine;

namespace BetterEditor.Commands
{
    [CommandInfo(Id = "page")]
    class Page : BECommand
    {
        public Page() : base()
        {

        }

        public override void Execute(scnEditor instance, string[] args)
        {
            int pageMargin = 0;

            if (args.Length == 0) return;

            if (args.Length > 1)
            {
                object p = castStringToType(args[1]);

                if (isType(p, out int page)) pageMargin = page;
            }

            int currentPage, maxPage;

            currentPage = scnEditorPrivates.GetField<int>("currentPage");
            maxPage = scnEditorPrivates.GetField<int>("maxPage");

            switch (args[0].ToLower())
            {
                case "set":
                    currentPage = Mathf.Clamp(pageMargin, 0, maxPage);
                    break;
                case "add":
                case "next":
                    currentPage = Mathf.Clamp(currentPage + pageMargin, 0, maxPage);
                    break;
                case "substract":
                case "previous":
                case "prev":
                    currentPage = Mathf.Clamp(currentPage - pageMargin, 0, maxPage);
                    break;
                default:
                    return;
            }

            scnEditorPrivates.SetField("currentPage", currentPage);
            instance.ShowEventsPage(currentPage);
        }
    }
}
