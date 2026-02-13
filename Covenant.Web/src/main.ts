import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createHead } from '@unhead/vue/client'

import App from './App.vue'
import router from './router'
import vuetify from './plugins/vuetify'

const app = createApp(App)
const head = createHead()

app.use(createPinia())
app.use(router)
app.use(vuetify)
app.use(head)

app.mount('#app')
