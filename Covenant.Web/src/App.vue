<template>
  <router-view />

  <!-- Global toast notification -->
  <v-snackbar
    v-model="toastState.show"
    :color="toastState.color"
    :timeout="toastState.timeout"
    location="top"
  >
    {{ toastState.message }}
  </v-snackbar>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useHead, useSeoMeta } from '@unhead/vue'
import { useToast } from '@/composables/useToast'

const SITE_ORIGIN = 'https://www.covenantgroupl.com'

const { toastState } = useToast()
const route = useRoute()

const canonicalUrl = computed(() => {
  const path = route.path === '/' ? '/' : route.path.replace(/\/+$/, '')
  return `${SITE_ORIGIN}${path}`
})

useHead({ link: [{ rel: 'canonical', href: canonicalUrl }] })
useSeoMeta({ ogUrl: canonicalUrl })
</script>
