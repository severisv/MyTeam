// js
var gulp = require("gulp");
var concat = require('gulp-concat');
var paths = require('./paths');
var browserify = require('browserify');
var notify = require('gulp-notify');
var source = require('vinyl-source-stream');
var _if = require('gulp-if');
var _ = require('./utils');
var isProduction = true;


gulp.task('js', function () {
    var options = {
        debug: !isProduction
    };

    var b = browserify(options);
    b.add(paths.src.scripts);
    b.add(paths.src.lib);
    b.bundle()
    .on('error', _.plumb.errorHandler)
    .pipe(source('site.js'))
    .pipe(gulp.dest(paths.dest.scripts))
    .pipe(notify('Compiled javascript'));
});


// Watch
gulp.task('watch-js', function () {
    gulp.watch(paths.src.scripts, ['js']);
});


