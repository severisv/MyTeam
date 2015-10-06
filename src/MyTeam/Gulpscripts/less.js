//  Less  
var less = require('gulp-less');
var gulp = require("gulp");
var paths = require('./paths');
var concat = require('gulp-concat');
var _ = require('./utils');
var notify = require('gulp-notify');


gulp.task('less', function () {
    return gulp.src(paths.src.stylesheets)
      .pipe(less())
      .pipe(concat('styles.css'))
        .on('error', _.plumb.errorHandler)
      .pipe(gulp.dest(paths.dest.stylesheets))
      .pipe(notify('Compiled Less'));
    
});



// Watch
gulp.task('watch-less', function () {
    gulp.watch(paths.src.stylesheets, ['less']);
});

