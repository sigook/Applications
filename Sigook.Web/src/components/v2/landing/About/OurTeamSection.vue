<template>
  <section class="team">
    <header class="team__header">
      <EyebrowPill variant="red" class="team__eyebrow">
        Our Team
      </EyebrowPill>

      <h2 class="team__heading">
        Meet the people
        <span class="team__heading-accent">behind Sigook.</span>
      </h2>

      <p class="team__subtitle">
        Founder-led and people-driven — the team turning trust and integrity
        into lasting partnerships for employers and workers alike.
      </p>
    </header>

    <div class="team__grid">
      <article v-for="member in MEMBERS" :key="member.name" class="team__card">
        <div class="team__photo">
          <img v-if="member.photo" :src="member.photo" :alt="member.name" />
          <span v-else class="team__initials" aria-hidden="true">{{ initials(member.name) }}</span>
        </div>

        <h3 class="team__name">{{ member.name }}</h3>
        <p class="team__role">{{ member.role }}</p>
        <p class="team__bio">{{ member.bio }}</p>

        <a
          v-if="member.linkedin"
          class="team__linkedin"
          :href="member.linkedin"
          target="_blank"
          rel="noopener noreferrer"
          :aria-label="`${member.name} on LinkedIn`"
        >
          <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
            <path d="M20.45 20.45h-3.56v-5.57c0-1.33-.03-3.04-1.85-3.04-1.86 0-2.14 1.45-2.14 2.94v5.67H9.35V9h3.41v1.56h.05c.48-.9 1.64-1.85 3.37-1.85 3.6 0 4.27 2.37 4.27 5.46v6.28zM5.34 7.43a2.07 2.07 0 1 1 0-4.14 2.07 2.07 0 0 1 0 4.14zM7.12 20.45H3.56V9h3.56v11.45zM22.22 0H1.77C.79 0 0 .78 0 1.73v20.54C0 23.23.79 24 1.77 24h20.45c.98 0 1.78-.77 1.78-1.73V1.73C24 .78 23.2 0 22.22 0z" />
          </svg>
          <span>LinkedIn</span>
        </a>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'

interface Member {
  readonly name: string
  readonly role: string
  readonly bio: string
  readonly linkedin: string
  readonly photo: string | null
}

const MEMBERS: readonly Member[] = [
  {
    name: 'Andrea',
    role: 'Co-Founder & CEO',
    bio: 'Majority owner of the Covenant Group family, Andrea guides the company\'s direction and long-term vision.',
    linkedin: '#',
    photo: null,
  },
  {
    name: 'David',
    role: 'Co-Founder & President',
    bio: 'Drives growth initiatives, strategic partnerships, and new ventures across staffing, technology, and workforce services.',
    linkedin: '#',
    photo: null,
  },
]

function initials(name: string): string {
  return name
    .trim()
    .split(/\s+/)
    .map((part) => part[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}
</script>

<style scoped>
.team {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(96px, 12vw, 180px);
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

.team__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto clamp(44px, 6vw, 72px);
}

.team__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.team__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0;
  max-width: 720px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.4);
}

.team__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.team__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: clamp(14px, 1.8vw, 20px) 0 0;
  max-width: 580px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.3);
}

.team__grid {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 320px));
  justify-content: center;
  gap: clamp(20px, 2.6vw, 32px);
  max-width: 1180px;
  margin: 0 auto;
}

.team__card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: clamp(8px, 1vw, 12px);
  padding: clamp(28px, 3.2vw, 40px) clamp(20px, 2.4vw, 28px);
  background: linear-gradient(160deg,
    rgba(0, 173, 239, 0.1) 0%,
    rgba(9, 48, 85, 0.4) 58%,
    rgba(9, 48, 85, 0.5) 100%);
  border: 1px solid rgba(255, 255, 255, 0.16);
  border-radius:
    clamp(20px, 2.4vw, 28px) clamp(20px, 2.4vw, 28px)
    clamp(20px, 2.4vw, 28px) clamp(40px, 5vw, 56px);
  backdrop-filter: blur(18px) saturate(160%);
  -webkit-backdrop-filter: blur(18px) saturate(160%);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.25);
  overflow: hidden;
  transition:
    transform 0.35s cubic-bezier(0.22, 1, 0.36, 1),
    border-color 0.3s ease,
    box-shadow 0.3s ease;
}

.team__card:hover {
  transform: translateY(-6px);
  border-color: rgba(0, 173, 239, 0.45);
  box-shadow:
    0 22px 44px rgba(0, 0, 0, 0.35),
    0 14px 34px -10px rgba(0, 173, 239, 0.35);
}

.team__photo {
  display: flex;
  align-items: center;
  justify-content: center;
  width: clamp(92px, 10vw, 120px);
  height: clamp(92px, 10vw, 120px);
  border-radius: 50%;
  overflow: hidden;
  border: 2px solid rgba(0, 173, 239, 0.5);
  background: linear-gradient(140deg, var(--c-brand-cyan), var(--c-brand-blue));
  box-shadow: 0 8px 22px rgba(0, 0, 0, 0.3);
}

.team__photo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.team__initials {
  font-size: clamp(30px, 3.4vw, 42px);
  font-weight: 800;
  letter-spacing: 0.02em;
  color: #fff;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
}

.team__name {
  font-size: clamp(16px, 1.6vw, 19px);
  font-weight: 700;
  color: #fff;
  margin: clamp(6px, 0.8vw, 10px) 0 0;
}

.team__role {
  font-size: clamp(11px, 1vw, 12px);
  font-weight: 700;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin: 0;
}

.team__bio {
  font-size: clamp(12.5px, 1.05vw, 14px);
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: clamp(6px, 0.8vw, 10px) 0 clamp(12px, 1.4vw, 16px);
}

.team__linkedin {
  margin-top: auto;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: clamp(8px, 1vw, 10px) clamp(16px, 1.8vw, 20px);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: #fff;
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 600;
  letter-spacing: 0.02em;
  text-decoration: none;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.team__linkedin:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateY(-1px);
}

.team__linkedin svg {
  width: 16px;
  height: 16px;
}
</style>
