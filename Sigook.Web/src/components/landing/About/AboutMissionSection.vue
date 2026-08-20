<template>
  <section class="about-mission">
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

import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'
import LandingSectionHeader from '@/components/landing/shared/sections/LandingSectionHeader.vue'
import SecondaryCard from '@/components/landing/shared/cards/SecondaryCard.vue'
</script>

<style scoped>
.about-mission {
  position: relative;
  width: 100%;
  padding:
    var(--section-pad-y)
    clamp(20px, 3vw, 40px)
    clamp(180px, 18vw, 300px);
  isolation: isolate;
  overflow: hidden;
}

.about-mission__magnifier {
  bottom: clamp(8%, 10vw, 14%);
  left: clamp(5%, 6vw, 8%);
}

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

@media (max-width: 1023px) {
  .about-mission__cards { grid-template-columns: 1fr; }

  .about-mission__card--offset { margin-top: 0; }

  .about-mission__magnifier {
    bottom: 6%;
    left: 6%;
  }
}
</style>
