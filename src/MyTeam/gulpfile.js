/// <binding AfterBuild='copy, less, js' Clean='clean' ProjectOpened='watch' />
var gulp = require("gulp"),
  rimraf = require("rimraf"),
  fs = require("fs"),
    concat = require('gulp-concat');

eval("var project = " + fs.readFileSync("./project.json"));

var paths = {
    root: "./" + project.webroot,
    bower: "./bower_components/",
    stylesheets: "./Assets/Stylesheets/*.less",
    scripts: "./Assets/Scripts/*.js"
};

var destPaths = {
    lib: paths.root + "/lib/",
    stylesheets: paths.root + "/css/",
    scripts: paths.root + "/scripts/"
}


// Clean
gulp.task("clean", function (cb) {
    rimraf(destPaths.scripts, function () { });
    rimraf(destPaths.stylesheets, function () { });
});


// js
var uglify = require('gulp-uglify');

gulp.task('js', function () {
    return gulp.src(paths.scripts)
        .pipe(concat('site.js'))
//      .pipe(uglify())
      .pipe(gulp.dest(destPaths.scripts));
});



//  Less  
var less = require('gulp-less');

gulp.task('less', function () {
    return gulp.src(paths.stylesheets)
      .pipe(less())
      .pipe(concat('styles.css'))
      .on('error', swallowError)
      .pipe(gulp.dest(destPaths.stylesheets));
});


// Watch
gulp.task('watch', function () {
    gulp.watch(paths.stylesheets, ['less']);
    gulp.watch(paths.scripts, ['js']);
});



// Helper
function swallowError(error) {

    console.log(error.toString());

    this.emit('end');
}