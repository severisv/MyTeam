const gulp = require('gulp')
const uglify = require('gulp-uglify')
const concat = require('gulp-concat')

const npmFolder = './node_modules/'
const wwwroot = './wwwroot'

function npm(item) {
  return npmFolder + item
}

const paths = {
  src: {
    lib: [
      npm('jquery/dist/jquery.js'),
      npm('bootstrap/dist/js/bootstrap.js'),
      npm('bootbox/bootbox.js'),
    ],
    cloudinary: [
      npm('jquery.cloudinary/js/jquery.ui.widget.js'),
      npm('jquery.cloudinary/js/jquery.iframe-transport.js'),
      npm('jquery.cloudinary/js/jquery.fileupload.js'),
      npm('jquery.cloudinary/js/jquery.cloudinary.js'),
    ],
  },
  dest: `${wwwroot}/compiled/lib/`,
}

gulp.task('js-lib', () =>
  gulp
    .src(paths.src.lib)
    .pipe(concat('lib.bundle.js'))
    .pipe(uglify())
    .pipe(gulp.dest(paths.dest)))

gulp.task('js-cloudinary', () =>
  gulp
    .src(paths.src.cloudinary)
    .pipe(concat('cloudinary.bundle.js'))
    .pipe(uglify())
    .pipe(gulp.dest(paths.dest)))

gulp.task('default', ['js-lib', 'js-cloudinary'])
