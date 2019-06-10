const path = require("path");
const webpack = require("webpack");

console.log("Bundling for development...");

var babelOptions = {
  presets: [["@babel/preset-env", { targets: { browsers: "> 1%" }, modules: false }]]
};

module.exports = {
  mode: "development",
  devtool: "source-map",
  entry: [ "./client/src/client.fsproj"],
  output: {
    filename: "[name].js",
    path: path.join(__dirname, "../server/wwwroot/compiled/client"),
    devtoolModuleFilenameTemplate: info =>
      path.resolve(info.absoluteResourcePath).replace(/\\/g, "/")
  },
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: {
            babel: babelOptions,
            define: ["DEBUG"]
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
  },
  plugins: [
      new webpack.NamedModulesPlugin()
    ]
};
