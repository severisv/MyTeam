const path = require('path')
const CheckerPlugin = require('awesome-typescript-loader')
const ExtractTextPlugin = require('extract-text-webpack-plugin')
const Wildcards = require('wildcards-entry-webpack-plugin')

module.exports = {
  entry: Wildcards.entry('./client/scripts/views/*/*.js', {
    app: ['babel-polyfill', 'whatwg-fetch', './client/scripts/app.js'],
  }),

  output: {
    path: path.resolve('./wwwroot/compiled/scripts'),
    filename: '[name].js',
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx', '.css'],
    modules: [path.resolve('./node_modules'), path.resolve('./')],
  },
  devtool: 'source-map',

  module: {
    loaders: [
      {
        test: /\.tsx?$/,
        loader: 'awesome-typescript-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.jsx?$/,
        loader: 'babel-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.(css|less)$/,
        use: ExtractTextPlugin.extract({
          use: [
            {
              loader: 'css-loader',
              options: { sourceMap: true, minimize: true },
            },
            {
              loader: 'less-loader',
              options: {
                options: { sourceMap: true },
              },
            },
          ],
        }),
      },
      {
        test: /\.(jpg|png|svg)$/,
        use: [
          {
            loader: 'file-loader',
            options: {
              hash: 'sha512',
              digest: 'hex',
              name: 'images/[name].[ext]',
            },
          },
        ],
      },
      {
        test: /\.(ttf|otf|eot|woff(2)?)(\?[a-z0-9]+)?$/,
        use: [
          {
            loader: 'file-loader',
            options: {
              hash: 'sha512',
              digest: 'hex',
              name: 'fonts/[name].[ext]',
            },
          },
        ],
      },
    ],
  },

  plugins: [
    new Wildcards(),
    new CheckerPlugin(),
    new ExtractTextPlugin({
      filename: 'site.bundle.css',
      allChunks: false,
    }),
  ],
}
