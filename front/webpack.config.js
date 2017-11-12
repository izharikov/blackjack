'use strict';

var path = require('path');
var webpack = require('webpack');

var WebpackCleanupPlugin = require('webpack-cleanup-plugin');
var HtmlWebpackHarddiskPlugin = require('html-webpack-harddisk-plugin');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = {
    context: path.join(__dirname, '/src'),
    entry: {
        app: './index.jsx',
        vendor: './vendor.js'
    },
    devtool: 'source-map',
    output: {
        path: path.join(__dirname, '/dist/'),
        filename: '[name].bundle.js',
        sourceMapFilename: '[name].map',
        publicPath: '/dist/'
    },
    resolve: {
        extensions: ['.js', '.jsx', '.scss']
    },
    module: {
        loaders: [
            // Transform JSX in .jsx files
            { test: /\.jsx$/, loader: 'babel-loader' },
            {
                test: /\.jsx?$/,         // Match both .js and .jsx files
                exclude: /node_modules/,
                loader: "babel-loader",
                query: {
                    presets: ["babel-preset-es2015", "babel-preset-es2016", "babel-preset-es2017", 'react', 'stage-2']
                }
            },
            {
                test: /\.css$/,
                exclude: /node_modules/,
                use: ExtractTextPlugin.extract({
                    fallback: 'style-loader',

                    // Could also be write as follow:
                    // use: 'css-loader?modules&localIdentName=[name]__[local]___[hash:base64:5]!postcss-loader'
                    use: [
                        {
                            loader: 'css-loader',
                            query: {
                                modules: true,
                                localIdentName: '[name]__[local]'
                            }
                        },
                        'postcss-loader'
                    ]
                }),
            },
            {
                test: /\.scss$/,
                exclude: /node_modules/,
                use: ExtractTextPlugin.extract({
                    fallback: 'style-loader',

                    // Could also be write as follow:
                    // use: 'css-loader?modules&importLoader=2&sourceMap&localIdentName=[name]__[local]___[hash:base64:5]!sass-loader'
                    use: [
                        {
                            loader: 'css-loader',
                            query: {
                                modules: true,
                                sourceMap: true,
                                importLoaders: 2,
                                localIdentName: '[name]__[local]'
                            }
                        },
                        'sass-loader'
                    ]
                }),
            },
            {
                test: /\.(jpg|png|svg)$/,
                loader: 'file-loader',
                options: {
                    name: '[path][name].[ext]',
                },
            }
        ]
    },
    plugins: [
        // new webpack.optimize.CommonsChunkPlugin({
        //     name: ['app', 'vendor']
        // }),
        new HtmlWebpackPlugin({
            title: 'Blackjack',
            template: path.join(__dirname, '/src/index.ejs'),
            filename: path.join(__dirname, '/index.html'),
            alwaysWriteToDisk: true
        }),
        // new WebpackCleanupPlugin(),
        new HtmlWebpackHarddiskPlugin(),
        new ExtractTextPlugin('styles.out.css'),
        new webpack.SourceMapDevToolPlugin({
            filename: "[file].map"
        }),
    ],
    devServer: {
        compress: true,
        port: 3000,
        historyApiFallback: true,
        proxy: {
            '/api': {
                target: 'http://localhost:5000',
                secure: false,
                pathRewrite: { '^/api': '' }
            }
        }
    }
};