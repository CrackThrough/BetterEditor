const fs = require("fs");
const VERSION = process.argv[2].slice().splice(2).join(" ");

["./Repository.json", "./Info.json"].forEach((filename, index) => {
    var data = require(filename);
    switch (index) {
        case 0:
            data.Releases.Version = VERSION;
            data.Releases.DownloadUrl = `https://github.com/CrackThrough/BetterEditor/releases/download/v${VERSION}/BetterEditor-${VERSION}.zip`;
            break;
        case 1:
            data.Version = VERSION;
            break;
    }
    fs.writeFileSync(filename, JSON.stringify(info, (k, v) => v, 4 / (index + 1)));
});