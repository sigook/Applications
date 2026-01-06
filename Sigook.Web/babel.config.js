const  plugins = [
  '@babel/plugin-transform-optional-chaining'
];

if (process.env.VUE_NODE_ENV === 'production') {
  plugins.push('transform-remove-console')
}

module.exports = {
  presets: [
    '@vue/app'
  ],
  plugins: plugins
};