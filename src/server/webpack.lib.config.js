const path = require('path');

module.exports = {
    entry: [
        './client/scripts/lib.js'
    ],
    output: {
        filename: 'lib.bundle.js',
        path: path.resolve(__dirname, 'wwwroot/compiled/lib')
    }
};