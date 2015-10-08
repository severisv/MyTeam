fs = require("fs"),

eval("var project = " + fs.readFileSync("./project.json"));
var join = require('path').resolve;
var root = "./" + project.webroot;
var bowerFolder = "./wwwroot/lib/";
var npmFolder = "./node_modules/";

function bower(item) {
    return bowerFolder + item;
}

function npm(item) {
    return npmFolder + item;
}

module.exports = {
    self: "./Gulpscripts/paths.js",

    src: {
        root: root,
        stylesheets: "./Assets/Stylesheets/styles.less",
        scripts: join("", './Assets/Scripts/*.{js,jsx}'),
        
        lib: [
            npm("jquery/dist/jquery.js"),
            bower("jquery-ui/jquery-ui.js"),
            bower("tablesorter/jquery.tablesorter.js"),
            bower("bootstrap/dist/js/bootstrap.js"),
            bower("jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.js"),
            npm("bootbox/bootbox.js"),
            npm("react/dist/react.js")
        ]
    },
    dest : {
        stylesheets: root + "/css/",
        scripts: root + "/scripts/"
    }
}