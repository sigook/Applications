<template>
  <section class="about-mission">
    <!-- Atmospheric magnifier decoration — bottom-left anchor -->
    <DecoMagnifier class="about-mission__magnifier" />

    <LandingSectionHeader
      eyebrow="Why We Exist"
      heading="Built on purpose."
      heading-accent="Driven by people."
      subtitle="Two questions guide everything we build — what we're here to do, and the future we're trying to make real."
      margin-bottom="clamp(48px, 6vw, 80px)"
      heading-line-height="1.1"
      subtitle-color="rgba(255, 255, 255, 0.75)"
    />

    <div class="about-mission__cards">
      <SecondaryCard
        variant="blue"
        eyebrow="Our Mission"
        title="Connect great people with great opportunities — fairly, transparently, and at the pace today's businesses move."
        :delay="0"
        class="about-mission__card"
      >
        We exist to remove the friction between talent and the companies that
        need them — replacing slow, opaque hiring with technology that respects
        everyone's time and treats every match as a relationship, not a
        transaction.
      </SecondaryCard>

      <SecondaryCard
        variant="red"
        eyebrow="Our Vision"
        title="An American workforce where every connection sparks real growth — for workers, companies, and the communities they share."
        :delay="180"
        class="about-mission__card about-mission__card--offset"
      >
        We see a future where staffing is built on trust, not transactions, and
        where opportunity travels as freely as the people who pursue it —
        across borders, industries, and generations.
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * About — Mission & Vision section.
 *
 * Two SecondaryCard cards (canonical Home pattern) declaring the company's
 * purpose. Mission pulls the blue/red-accent variant; Vision pulls the
 * red/blue-accent variant — keeping the brand duality on the page while
 * matching the visual vocabulary established by Home WhyChooseUs.
 *
 * Card 02 sits lower (margin-top offset) to break DualCta-style symmetry —
 * this is a declaration, not an action prompt.
 */
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import SecondaryCard from '@/components/landing/shared/SecondaryCard.vue'
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.about-mission {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(180px, 18vw, 300px);
  isolation: isolate;
  overflow: hidden;
}

.about-mission__magnifier {
  bottom: clamp(8%, 10vw, 14%);
  left: clamp(5%, 6vw, 8%);
}

/* ── Cards grid — 2 col with staggered offset on second card ────────────── */
.about-mission__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(24px, 3.5vw, 48px);
  align-items: start;
  max-width: 1180px;
  margin: 0 auto;
}

.about-mission__card--offset {
  margin-top: clamp(40px, 6vw, 96px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .about-mission__cards { grid-template-columns: 1fr; }

  .about-mission__card--offset { margin-top: 0; }

  .about-mission__magnifier {
    bottom: 6%;
    left: 6%;
  }
}
</style>
