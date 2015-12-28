fs = require("fs"),

eval("var project = " + fs.readFileSync("./project.json"));
var join = require('path').resolve;
var root = "./wwwroot";
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
        stylesheets: join("", './Assets/Stylesheets/*/*.{less,css}'),
        scripts: [
            join("", './Assets/Scripts/*.js'),
            join("", './Assets/Scripts/ReactComponents/*.jsx'),
            join("", './Assets/Scripts/ReactComponents/*/*.jsx')
        ],
        
        lib: [
            npm("jquery/dist/jquery.js"),
            bower("jquery-ui/jquery-ui.js"),
            bower("tablesorter/jquery.tablesorter.js"),
            bower("bootstrap/dist/js/bootstrap.js"),
            bower("jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.js"),
            npm("bootbox/bootbox.js"),
            npm("react/dist/react.js")
        ],
        cloudinary: [
            npm("jquery.cloudinary/js/jquery.ui.widget.js"),
            npm("jquery.cloudinary/js/jquery.iframe-transport.js"),
            npm("jquery.cloudinary/js/jquery.fileupload.js"),
            npm("jquery.cloudinary/js/jquery.cloudinary.js")
        ]
    },
    dest : {
        stylesheets: root + "/css/",
        scripts: root + "/scripts/Bundles/"
    }
}