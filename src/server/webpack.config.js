const path = require('path')
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const TerserJSPlugin = require('terser-webpack-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');

module.exports = {
  entry: {
    app: ['babel-polyfill', 'whatwg-fetch', './client/scripts/app.js',  './client/scripts/views/listEvents.js']
  },

  output: {
    path: path.resolve('./wwwroot/compiled'),
    filename: '[name].js',
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx', '.css'],
    modules: [path.resolve('./node_modules'), path.resolve('./')],
  },
  devtool: 'source-map',

  module: {
    rules: [
      {
        test: /\.jsx?$/,
        loader: 'babel-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.(css|less)$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
            options: {    
              hmr: process.env.NODE_ENV === 'development'
            },
          },
          'css-loader',
          { loader: 'less-loader', options: { sourceMap: true }}
        ]
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
              name: 'fonts/[name].[ext]'
            },
          },
        ],
      },
    ]
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: 'site.bundle.css'
    })
  ],
  optimization: {
    minimizer: [new TerserJSPlugin({}), new OptimizeCSSAssetsPlugin({})],
  }
}
