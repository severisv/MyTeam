const join = require('path').resolve

const root = './wwwroot'
const bowerFolder = './wwwroot/lib/'
const npmFolder = './node_modules/'

function bower(item) {
  return bowerFolder + item
}

function npm(item) {
  return npmFolder + item
}

module.exports = {
  self: './Gulpscripts/paths.js',

  src: {
    root,
    stylesheet: join('', './Assets/Stylesheets/style.less'),
    stylesheets: join('', './Assets/Stylesheets/*/*.{less,css}'),
    scripts: [
      join('', './Assets/Scripts/*.js'),
      join('', './Assets/Scripts/ReactComponents/*.jsx'),
      join('', './Assets/Scripts/ReactComponents/*/*.jsx'),
    ],
    scriptsRoot: './Assets/Scripts/app.js',

    lib: [
      npm('jquery/dist/jquery.js'),
      bower('tablesorter/jquery.tablesorter.js'),
      bower('bootstrap/dist/js/bootstrap.js'),
      npm('bootbox/bootbox.js'),
    ],
    cloudinary: [
      npm('jquery.cloudinary/js/jquery.ui.widget.js'),
      npm('jquery.cloudinary/js/jquery.iframe-transport.js'),
      npm('jquery.cloudinary/js/jquery.fileupload.js'),
      npm('jquery.cloudinary/js/jquery.cloudinary.js'),
    ],
  },
  dest: {
    stylesheets: `${root}/css/`,
    scripts: `${root}/scripts/Bundles/`,
  },
}
