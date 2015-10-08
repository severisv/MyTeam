// js
var gulp = require("gulp");
var concat = require('gulp-concat');
var paths = require('./paths');
var notify = require('gulp-notify');
var _ = require('./utils');
var uglify = require('gulp-uglify');
var react = require('gulp-react');


gulp.task('js', function () {
    return gulp.src(paths.src.scripts)
      .pipe(concat('site.bundle.js'))
      .on('error', _.plumb.errorHandler)
      .pipe(react())
//      .pipe(uglify())
      .pipe(gulp.dest(paths.dest.scripts))
      .pipe(notify('Compiled javascript'));
    
});

gulp.task('js-lib', function () {
    return gulp.src(paths.src.lib)
        .pipe(concat('lib.bundle.js'))
        .on('error', _.plumb.errorHandler)
        .pipe(uglify())
        .pipe(gulp.dest(paths.dest.scripts))
        .pipe(notify('Compiled javascript libraries'));
    
});


// Watch
gulp.task('watch-js', function () {
    gulp.watch(paths.src.scripts, ['js']);
});

gulp.task('watch-js-lib', function () {
    gulp.watch(paths.self, ['js-lib']);
});


