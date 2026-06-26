<template>
  <section class="team">
    <LandingSectionHeader
      eyebrow="Our Team"
      eyebrow-variant="red"
      heading="Meet the people"
      heading-accent="behind Sigook."
      subtitle="Founder-led and people-driven — the team turning trust and integrity into lasting partnerships for employers and workers alike."
      heading-max-width="720px"
      heading-margin-bottom="0"
      margin-bottom="clamp(44px, 6vw, 72px)"
      subtitle-max-width="580px"
      subtitle-margin-top="clamp(14px, 1.8vw, 20px)"
    />

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
          v-if="hasLinkedin(member)"
          class="team__linkedin"
          :href="member.linkedin"
          target="_blank"
          rel="noopener noreferrer"
          :aria-label="`${member.name} on LinkedIn`"
        >
          <LandingIcon name="linkedin" />
          <span>LinkedIn</span>
        </a>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
import LandingIcon from '@/components/landing/shared/LandingIcon.vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'

interface Member {
  readonly name: string
  readonly role: string
  readonly bio: string
  readonly linkedin?: string
  readonly photo: string | null
}

const MEMBERS: readonly Member[] = [
  {
    name: 'Andrea Gonzalez',
    role: 'Co-Founder & CEO',
    bio: 'Majority owner of the Covenant Group family, Andrea guides the company\'s direction and long-term vision.',
    linkedin: 'https://www.linkedin.com/in/andreagonzalesgm/',
    photo: null,
  },
  {
    name: 'David Ballesteros',
    role: 'Co-Founder & Business Director',
    bio: 'Drives growth initiatives, strategic partnerships, and new ventures across staffing, technology, and workforce services.',
    linkedin: 'https://www.linkedin.com/in/saulo-david-ballesteros-8391513/',
    photo: null,
  },
  {
  name: 'Isabella Zabaleta',
    role: 'Operations and Management Support Assistant',
    bio: 'Provides essential support to our operations and management teams, ensuring smooth workflows and efficient processes across the organization.',
    linkedin: 'https://www.linkedin.com/in/isabella-zabaleta-marketing-ux/',
    photo: null,
  },
  {
    name: 'Leonardo Gomez',
    role: 'Recruiter & Talent Acquisition',
    bio: 'Skilled recruiter and talent acquisition specialist, dedicated to connecting top talent with the right opportunities within our organization.',
    linkedin: 'https://www.linkedin.com/in/leonardo-gomez-22279b23a/',
    photo: null,
  },
  {
    name: 'Daniela Garcia',
      role: 'Recruiter & Talent Acquisition',
      bio: 'Skilled recruiter and talent acquisition specialist, dedicated to connecting top talent with the right opportunities within our organization.',
      linkedin: 'https://www.linkedin.com/in/danielagarciasaave',
      photo: null,
  },
  {
    name: 'Carol Vargas',
      role: 'Recruiter & Talent Acquisition',
      bio: 'Skilled recruiter and talent acquisition specialist, dedicated to connecting top talent with the right opportunities within our organization.',
      linkedin: '#',
      photo: null,
  },
  {
    name: 'Indira Martinez',
      role: 'Payroll Specialist',
      bio: 'Skilled recruiter and talent acquisition specialist, dedicated to connecting top talent with the right opportunities within our organization.',
      linkedin: 'https://www.linkedin.com/in/indira-yasmin-martinez-rubiano-3247a978/',
      photo: null,
  },
  {
    name: 'Juan González',
      role: 'CTO - Lead Developer',
      bio: 'Experienced technology leader and fullstack developer, responsible for overseeing our technical strategy and leading the development of innovative solutions that drive our business forward.',
      linkedin: '#',
      photo: null,
  },
  {
    name: 'Juan Betancur',
      role: 'Senior Fullstack Developer',
      bio: 'Experienced fullstack developer with a strong focus on building scalable and maintainable web applications.',
      linkedin: 'https://www.linkedin.com/in/juan-betancur-b868b2213',
      photo: null,
  }
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

function hasLinkedin(member: Member): boolean {
  const url = member.linkedin?.trim()
  return !!url && url !== '#'
}
</script>

<style scoped>
.team {
  position: relative;
  width: 100%;
  padding:
    var(--section-pad-y)
    clamp(20px, 3vw, 40px)
    clamp(96px, 12vw, 180px);
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
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
    transform 0.35s var(--ease-brand),
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
