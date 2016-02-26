//  Less  
var less = require('gulp-less');
var gulp = require("gulp");
var paths = require('./paths');
var _ = require('./utils');
var sourcemaps = require('./utils/sourcemaps.js');
var notify = require('gulp-notify');
var minify = require('gulp-cssmin');
var args = require('yargs').argv;
var gif = require('gulp-if');
var isProduction = args.production;

gulp.task('less', function () {
    return gulp.src(paths.src.stylesheet)
      .pipe(gif(!isProduction, sourcemaps.init()))
      .pipe(less())
       .on('error', _.plumb.errorHandler)
       .pipe(gif(isProduction, minify()))
      .pipe(gif(!isProduction, sourcemaps.write('.')))
      .pipe(gulp.dest(paths.dest.stylesheets))
      .pipe(notify('Compiled Less'));
});


// Watch
gulp.task('watch-less', function () {
    gulp.watch(paths.src.stylesheets, ['less']);
});

