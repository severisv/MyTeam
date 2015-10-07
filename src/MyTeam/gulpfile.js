/// <binding AfterBuild='copy, less, js' Clean='clean' ProjectOpened='watch' />
var gulp = require("gulp"),
    rimraf = require("rimraf");

require('./Gulpscripts/js');
require('./Gulpscripts/less');
var paths = require('./Gulpscripts/paths');


gulp.task('default', ['less', 'js', 'js-lib']);
gulp.task('watch', ['watch-js', 'watch-js-lib', 'watch-less']);


gulp.task("clean", function (cb) {
    rimraf(paths.dest.scripts, function () { });
    rimraf(paths.dest.stylesheets, function () { });
});




