//  Less
var less = require('gulp-less');
var gulp = require('gulp');
var paths = require('./paths');
var _ = require('./utils');
var sourcemaps = require('gulp-sourcemaps');
var notify = require('gulp-notify');
var minify = require('gulp-csso');

gulp.task('less', function () {
    return gulp.src(paths.src.stylesheet)
      .pipe(sourcemaps.init())
      .pipe(less())
       .on('error', _.plumb.errorHandler)
      .pipe(minify())
      .pipe(sourcemaps.write('.'))
      .pipe(gulp.dest(paths.dest.stylesheets))
      .pipe(notify('Compiled Less'));
});


// Watch
gulp.task('watch-less', function () {
    gulp.watch(paths.src.stylesheets, ['less']);
});

