var args = require('yargs').argv;
var isProduction = !!args.production;

if(!isProduction){
     module.exports = require('gulp-sourcemaps');
}
else {
     module.exports = {
         init: function(){return {};},
         write: function(){return {};}
     };
}