var svgPath = "C:\\Users\\Weari\\Documents\\Dear Hiring Manager PC\\Marketing\\Logos\\DearHiringManager_Icon_v3.svg";
var aiSavePath = "C:\\Users\\Weari\\Documents\\Dear Hiring Manager PC\\Marketing\\Logos\\DearHiringManager_Icon_v3.ai";

try {
    var svgFile = new File(svgPath);
    if (svgFile.exists) {
        var doc = app.open(svgFile);
        var aiFile = new File(aiSavePath);
        var saveOptions = new IllustratorSaveOptions();
        saveOptions.compatibility = Compatibility.ILLUSTRATOR24;
        saveOptions.flattenOutput = OutputFlattening.PRESERVEAPPEARANCE;
        doc.saveAs(aiFile, saveOptions);
        "SUCCESS: Saved " + aiSavePath;
    } else {
        "ERROR: SVG not found at " + svgPath;
    }
} catch (e) {
    "ERROR: " + e.message;
}
