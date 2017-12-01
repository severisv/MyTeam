// js
var gulp = require('gulp');
var paths = require('./paths');
var notify = require('gulp-notify');
var _ = require('./utils');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');

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


