const BundleAnalizer = require("webpack-bundle-analyzer");

module.exports = {
  outputDir: "wwwroot",
  devServer: {
    port: 3001,
    public: "http://localhost:3001",
  },
  productionSourceMap: false,
  configureWebpack: (config) => {
    if (process.env.NODE_ENV !== "production") {
      return {
        output: {
          jsonpFunction: "webpackJsonpFunction3",
        },
        plugins: [
          new BundleAnalizer.BundleAnalyzerPlugin({
            analyzerMode: "static",
            openAnalyzer: false
          }),
        ],
      };
    } else {
      return { 
        output: { jsonpFunction: "webpackJsonpFunction3" }
      };
    }
  },
  chainWebpack: (config) => {
    // Configuración de chunks optimizados para code splitting
    if (process.env.NODE_ENV === 'production') {
      config.optimization.splitChunks({
        chunks: 'all',
        maxInitialRequests: Infinity,
        minSize: 20000,
        maxSize: 244000,
        cacheGroups: {
          // Chunk para vendor libraries principales
          vendor: {
            test: /[\\/]node_modules[\\/]/,
            name: 'vendor',
            chunks: 'all',
            priority: 20,
            enforce: true,
          },
          // Chunk específico para Vue ecosystem
          vue: {
            test: /[\\/]node_modules[\\/](vue|vue-router|vuex|@vue)[\\/]/,
            name: 'vue-vendor',
            chunks: 'all',
            priority: 30,
            enforce: true,
          },
          // Chunk para UI libraries (Buefy, Bootstrap)
          ui: {
            test: /[\\/]node_modules[\\/](buefy|bootstrap|@fortawesome)[\\/]/,
            name: 'ui-vendor',
            chunks: 'all',
            priority: 25,
            enforce: true,
          },
          // Chunks por módulos de la aplicación
          agency: {
            test: /[\\/]src[\\/]pages[\\/]agency[\\/]/,
            name: 'agency-chunk',
            chunks: 'all',
            priority: 10,
            minChunks: 1,
          },
          company: {
            test: /[\\/]src[\\/]pages[\\/]company[\\/]/,
            name: 'company-chunk',
            chunks: 'all',
            priority: 10,
            minChunks: 1,
          },
          worker: {
            test: /[\\/]src[\\/]pages[\\/]worker[\\/]/,
            name: 'worker-chunk',
            chunks: 'all',
            priority: 10,
            minChunks: 1,
          },
          landing: {
            test: /[\\/]src[\\/]pages[\\/]landing[\\/]/,
            name: 'landing-chunk',
            chunks: 'all',
            priority: 10,
            minChunks: 1,
          },
          // Chunk para componentes comunes
          common: {
            test: /[\\/]src[\\/]components[\\/]/,
            name: 'common-components',
            chunks: 'all',
            priority: 5,
            minChunks: 2,
          },
          // Chunk por defecto
          default: {
            minChunks: 2,
            priority: -20,
            reuseExistingChunk: true,
          },
        },
      });
    }
  },
};
