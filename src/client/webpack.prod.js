const path = require("path");

console.log("Bundling for production...");

var babelOptions = {
  presets: [["@babel/preset-env", { targets: { browsers: "> 1%" }, modules: false }]]
}; 

module.exports = {
  mode: "production",
  entry: [ "./client/src/client.fsproj"],
  output: {
    filename: "[name].js",
    path: path.join(__dirname, "../server/wwwroot/compiled/client")
  },
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: {
            babel: babelOptions
          }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: "babel-loader",
          options: babelOptions
        }
      }
    ]
  }
};
