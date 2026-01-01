import { ApplicationInsights } from '@microsoft/applicationinsights-web'

function install(Vue, options) {

    const config = options.appInsightsConfig || {};
    config.instrumentationKey = config.instrumentationKey || options.id;
    if (!config.instrumentationKey) return;

    if (options.appInsights) {
        Vue.appInsights = options.appInsights
    } else {
        Vue.appInsights = new ApplicationInsights({ config })
        Vue.appInsights.loadAppInsights()
        if (typeof (options.onAfterScriptLoaded) === 'function') {
            options.onAfterScriptLoaded(Vue.appInsights)
        }
    }

    Object.defineProperty(Vue.prototype, '$appInsights', {
        get: () => Vue.appInsights
    })
}

export default install