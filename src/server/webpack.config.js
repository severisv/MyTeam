const path = require('path')
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const TerserJSPlugin = require('terser-webpack-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = {
  entry: {
    app: [ './client/scripts/app.js']
  },

  output: {
    path: path.resolve('./wwwroot/compiled'),
    filename: '[name].js',
  },
  resolve: {
    extensions: ['.js', '.css'],
    modules: [path.resolve('./node_modules'), path.resolve('./')],
  },
  devtool: 'source-map',

  module: {
    rules: [
      {
        test: /\.js$/,
        loader: 'babel-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.(css|less)$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
          },
          'css-loader',
          { loader: 'less-loader', options: { sourceMap: true }}
        ]
      },
      {
        test: /\.(jpg|png|svg)$/,
        type: 'asset/resource',
        generator: {
            filename: 'images/[name].[ext]',
        }      
      },
      {
        test: /\.(ttf|otf|eot|woff(2)?)(\?[a-z0-9]+)?$/,
        type: 'asset/resource',
        generator: {
            filename: 'fonts/[name].[ext]'
        }      
      }
    ]
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: 'site.bundle.css'
    })
  ],
  optimization: {
    minimizer: [new TerserJSPlugin({}),new CssMinimizerPlugin()],
  }
}
