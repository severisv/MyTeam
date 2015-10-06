var join = require('path').resolve;

var notify = require('gulp-notify');


module.exports = {
    plumb: {
        errorHandler: notify.onError('Error: <%- error.message %>')
    }
  
};