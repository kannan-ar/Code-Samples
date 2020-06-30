const path = require('path');
var HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
    mode: "development",
    output: {
        filename: 'build/app.js',
        path: path.resolve("./dist"),
    },
    devtool: "source-map",
    resolve: {
        extensions: [".ts", ".tsx", '.js']
    },
    module: {
        rules: [
            {
                test: /\.ts(x?)$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: "ts-loader"
                    }
                ]
            },
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            }
        ]
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: 'public/index.html'
        }),
        new CopyWebpackPlugin({
            patterns: [
              { from: './node_modules/react/umd/react.development.js', to: './build/react.development.js' },
              { from: './node_modules/react-dom/umd/react-dom.development.js', to: './build/react-dom.development.js' },
            ],
          })
    ],
    externals: {
        "react": "React",
        "react-dom": "ReactDOM"
    }
}