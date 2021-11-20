const path = require("path");

console.log("Bundling for development...");

module.exports = {
  mode: "development",
  devtool: "source-map",
  entry: [ "./src/App.fs.js"],
  output: {
    filename: "[name].js",
    path: path.join(__dirname, "../server/wwwroot/compiled/client"),
  },
  module: {
 
  }
};
