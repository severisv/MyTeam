const path = require("path");
const webpack = require("webpack");
const common = require("./webpack.common");

console.log("Bundling for development...");

module.exports = {
  mode: "development",
  devtool: "source-map",
  entry: common.config.entry,
  output: {
    filename: "[name].js",
    path: common.config.buildDir,
    devtoolModuleFilenameTemplate: info =>
      path.resolve(info.absoluteResourcePath).replace(/\\/g, "/")
  },
  module: {
    rules: common.getModuleRules()
  },
  plugins: common
    .getPlugins()
    .concat([
      new webpack.HotModuleReplacementPlugin(),
      new webpack.NamedModulesPlugin()
    ]),
  resolve: {
    modules: [common.config.nodeModulesDir]
  }
};

module.exports.serve = {
  content: [common.config.rootDir]
};
