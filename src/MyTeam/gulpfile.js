const gulp = require('gulp')
const notify = require('gulp-notify')
const uglify = require('gulp-uglify')
const concat = require('gulp-concat')
const _ = require('./gulpscripts/utils')
const paths = require('./gulpscripts/paths')


gulp.task('js-lib', () => gulp
  .src(paths.src.lib)
  .pipe(concat('lib.bundle.js'))
  .pipe(uglify())
  .pipe(gulp.dest(paths.dest.scripts))
  .on('error', _.plumb.errorHandler)
  .pipe(notify('Compiled javascript libraries')))

gulp.task('js-cloudinary', () => gulp
  .src(paths.src.cloudinary)
  .pipe(concat('cloudinary.bundle.js'))
  .pipe(uglify())
  .pipe(gulp.dest(paths.dest.scripts))
  .on('error', _.plumb.errorHandler)
  .pipe(notify('Compiled Cloudinary javascript')))



gulp.task('default', ['js-lib', 'js-cloudinary'])
gulp.task('watch', ['default', 'watch-js', 'watch-js-lib', 'watch-less'])
