// js
var gulp = require("gulp");
var concat = require('gulp-concat');
var paths = require('./paths');
var notify = require('gulp-notify');
var _ = require('./utils');
var uglify = require('gulp-uglify');
var react = require('gulp-react');
var args = require('yargs').argv;
var gif = require('gulp-if');

var isProduction = args.production;

gulp.task('js', function () {
    return gulp.src(paths.src.scripts)
      .pipe(concat('site.bundle.js'))
      .pipe(react())
       .on('error', _.plumb.errorHandler)
      .pipe(gif(isProduction, uglify()))
      .on('error', _.plumb.errorHandler)
      .pipe(gulp.dest(paths.dest.scripts))
      .on('error', _.plumb.errorHandler)
      .pipe(notify('Compiled javascript'));
    
});

gulp.task('js-lib', function () {
    return gulp.src(paths.src.lib)
        .pipe(concat('lib.bundle.js'))
        .pipe(uglify())
        .pipe(gulp.dest(paths.dest.scripts))
        .on('error', _.plumb.errorHandler)
        .pipe(notify('Compiled javascript libraries'));
    
});

gulp.task('js-cloudinary', function () {
    return gulp.src(paths.src.cloudinary)
        .pipe(concat('cloudinary.bundle.js'))
        .pipe(uglify())
        .pipe(gulp.dest(paths.dest.scripts))
        .on('error', _.plumb.errorHandler)
        .pipe(notify('Compiled Cloudinary javascript'));
    
});


// Watch
gulp.task('watch-js', function () {
    gulp.watch(paths.src.scripts, ['js'])
            .on('error', _.plumb.errorHandler);
});

gulp.task('watch-js-lib', function () {
    gulp.watch(paths.self, ['js-lib'])
            .on('error', _.plumb.errorHandler);
});


