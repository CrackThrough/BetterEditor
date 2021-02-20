const fs = require("fs");
fs.copyFileSync(
    "./info.json",
    "C:/Program Files (x86)/Steam/steamapps/common/A Dance of Fire and Ice/Mods/BetterEditor/info.json"
);
fs.copyFileSync(
    "./bin/Debug/BetterEditor.dll",
    "C:/Program Files (x86)/Steam/steamapps/common/A Dance of Fire and Ice/Mods/BetterEditor/BetterEditor.dll"
);
