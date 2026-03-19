const BundleAnalizer = require("webpack-bundle-analyzer");

module.exports = {
  outputDir: "wwwroot",
  devServer: {
    port: 3001,
    client: {
      webSocketURL: "ws://localhost:3001/ws",
    },
  },
  productionSourceMap: false,
  css: {
    loaderOptions: {
      sass: {
        api: "modern-compiler",
        sassOptions: {
          silenceDeprecations: ["import"],
        },
      },
      scss: {
        api: "modern-compiler",
        sassOptions: {
          silenceDeprecations: ["import"],
        },
      },
    },
  },
  configureWebpack: (config) => {
    const base = {
      output: { chunkLoadingGlobal: "webpackJsonpFunction3" },
    };

    if (process.env.NODE_ENV === "production") {
      base.performance = { hints: false };
    }

    if (process.env.ANALYZE === "true") {
      base.plugins = [
        new BundleAnalizer.BundleAnalyzerPlugin({
          analyzerMode: "static",
          openAnalyzer: false,
        }),
      ];
    }

    return base;
  },
  chainWebpack: (config) => {
    // Skip css-loader URL resolution for absolute paths (served from public/)
    ['css', 'scss', 'sass'].forEach((lang) => {
      const rule = config.module.rule(lang);
      ['vue-modules', 'vue', 'normal-modules', 'normal'].forEach((type) => {
        const oneOf = rule.oneOf(type);
        if (oneOf.uses.has('css-loader')) {
          oneOf.use('css-loader').tap((options) => {
            options.url = {
              filter: (url) => !url.startsWith('/assets/'),
            };
            return options;
          });
        }
      });
    });

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
