<template>
  <main>
    <PartnerHeroSection />
    <PartnerTypesSection />
    <PartnerBenefitsSection />
    <PartnerProcessSection />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="partner-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
/**
 * /v2/partner landing page — Become a Partner.
 *
 * Section rhythm: window → panel → window → panel → window (contact)
 *  1. PartnerHeroSection      — Live story: bottom-center magnifier + stats
 *  2. PartnerTypesSection     — Two PrimaryCards (Recruiter / Business)
 *  3. PartnerBenefitsSection  — 4 SecondaryCards (Autonomy/Reach/Licensed/Reward)
 *  4. PartnerProcessSection   — 4 numbered steps (Apply → Onboard → Match → Earn)
 *  5. ContactSection (shared) — "Get in Touch" form
 */
import PartnerHeroSection from '@/components/v2/landing/Partner/PartnerHeroSection.vue'
import PartnerTypesSection from '@/components/v2/landing/Partner/PartnerTypesSection.vue'
import PartnerBenefitsSection from '@/components/v2/landing/Partner/PartnerBenefitsSection.vue'
import PartnerProcessSection from '@/components/v2/landing/Partner/PartnerProcessSection.vue'
import ContactSection from '@/components/v2/landing/shared/ContactSection.vue'
</script>
