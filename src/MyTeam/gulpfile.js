/// <binding ProjectOpened='watch' />
var gulp = require("gulp");
var rimraf = require("rimraf");

require('./Gulpscripts/js');
require('./Gulpscripts/less');
var paths = require('./Gulpscripts/paths');


gulp.task('default', ['less', 'js', 'js-lib']);
gulp.task('watch', ['default', 'watch-js', 'watch-js-lib', 'watch-less']);


gulp.task("clean", function (cb) {
    rimraf(paths.dest.scripts, function () { });
    rimraf(paths.dest.stylesheets, function () { });
});




