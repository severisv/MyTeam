const path = require("path");

console.log("Bundling for production...");


module.exports = {
  mode: "production",
  entry: [ "./src/App.fs.js"],
  output: {
    filename: "[name].js",
    path: path.join(__dirname, "../server/wwwroot/compiled/client")
  },
  module: {
  
  }
};
