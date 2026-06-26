<template>
  <section id="industries-grid" class="industries-grid">
    <div class="industries-grid__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Sectors We Serve"
      heading="Multiple industries."
      heading-accent="One commitment to specialized talent."
      subtitle="Explore the sectors where Sigook has the deepest expertise — tap any card to see the roles we place and how we partner."
    />

    <div class="industries-grid__grid">
      <IndustryCard
        v-for="(industry, idx) in INDUSTRIES"
        :key="industry.key"
        :industry="industry"
        :expanded="expandedKey === industry.key"
        :index="idx"
        @toggle="toggle(industry.key)"
      />
      <router-link
        to="#industries-contact"
        class="industries-grid__plus"
        aria-label="Don't see your sector? Talk to us"
      >
        <span class="industries-grid__plus-symbol" aria-hidden="true">+</span>
        <span class="industries-grid__plus-label">Don't see your sector?</span>
        <span class="industries-grid__plus-cta">Talk to us →</span>
      </router-link>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import IndustryCard, { type Industry } from '@/components/landing/Industries/IndustryCard.vue'

const INDUSTRIES: readonly Industry[] = [
  {
    key: 'automotive',
    title: 'Automotive',
    tagline: 'Powering mobility across the country.',
    body:
      'From assembly lines to dealership service bays, we place the technicians, inspectors, and operations staff that keep America\'s automotive ecosystem moving.',
    positions: [
      'Mechanical Technicians',
      'Quality Inspectors',
      'Service Advisors',
      'Production Operators',
    ],
    photo: 'https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=800&q=80&auto=format&fit=crop',
    tone: 'navy',
  },
  {
    key: 'aviation',
    title: 'Aviation',
    tagline: 'Keeping the skies safe and on schedule.',
    body:
      'Specialized recruiting for aircraft mechanics, ground crew, and avionics technicians — partnering with carriers, MROs, and airports across the country.',
    positions: [
      'Aircraft Mechanics',
      'Ground Crew',
      'Avionics Technicians',
      'Flight Operations',
    ],
    photo: 'https://images.unsplash.com/photo-1556388158-158ea5ccacbd?w=800&q=80&auto=format&fit=crop',
    tone: 'red',
  },
  {
    key: 'construction',
    title: 'Construction',
    tagline: 'Building the infrastructure America runs on.',
    body:
      'Skilled trades and project leadership for residential, commercial, and civil construction — from supervisor to safety officer, from crane operator to carpenter.',
    positions: [
      'Project Managers',
      'Site Supervisors',
      'Skilled Trades',
      'Safety Officers',
    ],
    photo: 'https://images.unsplash.com/photo-1581094288338-2314dddb7ece?w=800&q=80&auto=format&fit=crop',
    tone: 'navy',
  },
  {
    key: 'engineering',
    title: 'Engineering',
    tagline: 'Designing the systems that shape modern industry.',
    body:
      'Mechanical, electrical, civil, and process engineers placed across manufacturing, energy, and infrastructure projects requiring deep technical fluency.',
    positions: [
      'Mechanical Engineers',
      'Electrical Engineers',
      'Civil Engineers',
      'Process Engineers',
    ],
    photo: 'https://images.unsplash.com/photo-1581092335397-9583eb92d232?w=800&q=80&auto=format&fit=crop',
    tone: 'red',
  },
  {
    key: 'technology',
    title: 'Technology & IT',
    tagline: 'Talent for the engines of the digital economy.',
    body:
      'From software engineering to DevOps and data, we connect technology teams with builders who match their stack, pace, and ambition.',
    positions: [
      'Software Engineers',
      'DevOps Specialists',
      'Data Engineers',
      'IT Support Leads',
    ],
    photo: 'https://images.unsplash.com/photo-1518770660439-4636190af475?w=800&q=80&auto=format&fit=crop',
    tone: 'navy',
  },
  {
    key: 'finance',
    title: 'Finance',
    tagline: 'Where precision and trust drive every decision.',
    body:
      'Accounting, financial analysis, audit, and compliance professionals for firms that need numbers people they can count on — at every level of seniority.',
    positions: [
      'Accountants',
      'Financial Analysts',
      'Auditors',
      'Compliance Officers',
    ],
    photo: 'https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=800&q=80&auto=format&fit=crop',
    tone: 'red',
  },
  {
    key: 'legal',
    title: 'Legal',
    tagline: 'Supporting firms with specialist legal expertise.',
    body:
      'Paralegals, legal assistants, and compliance specialists for law firms, in-house teams, and regulatory functions that demand precision and confidentiality.',
    positions: [
      'Paralegals',
      'Legal Assistants',
      'Compliance Specialists',
      'Contract Coordinators',
    ],
    photo: 'https://images.unsplash.com/photo-1589994965851-a8f479c573a9?w=800&q=80&auto=format&fit=crop',
    tone: 'navy',
  },
  {
    key: 'logistics',
    title: 'Logistics',
    tagline: 'Moving goods, people, and possibility.',
    body:
      'End-to-end logistics talent — from warehouse floor to dispatch desk to inventory planning — for distributors, 3PLs, and supply chain operations.',
    positions: [
      'Warehouse Coordinators',
      'Dispatchers',
      'Inventory Specialists',
      'Supply Chain Analysts',
    ],
    photo: 'https://images.unsplash.com/photo-1553413077-190dd305871c?w=800&q=80&auto=format&fit=crop',
    tone: 'red',
  },
  {
    key: 'manufacturing',
    title: 'Manufacturing',
    tagline: 'The backbone of American production.',
    body:
      'Machine operators, line leads, and quality professionals for plants running everything from food processing to advanced materials and CNC fabrication.',
    positions: [
      'Machine Operators',
      'Production Supervisors',
      'Quality Technicians',
      'Maintenance Techs',
    ],
    photo: 'https://images.unsplash.com/photo-1565793298595-6a879b1d9492?w=800&q=80&auto=format&fit=crop',
    tone: 'navy',
  },
  {
    key: 'retail',
    title: 'Retail',
    tagline: 'Connecting brands with the people who power them.',
    body:
      'Store managers, merchandising leads, and customer-facing staff for retailers scaling from single-location operations to multi-state networks.',
    positions: [
      'Store Managers',
      'Merchandising Specialists',
      'Customer Service Leads',
      'Inventory Associates',
    ],
    photo: 'https://images.unsplash.com/photo-1481437156560-3205f6a55735?w=800&q=80&auto=format&fit=crop',
    tone: 'red',
  },
  {
    key: 'transportation',
    title: 'Transportation',
    tagline: 'Keeping commerce moving across roads and rails.',
    body:
      'Licensed drivers, fleet coordinators, and operations professionals for the transport companies that knit America\'s commerce together.',
    positions: [
      'CDL Drivers',
      'Fleet Coordinators',
      'Operations Managers',
      'Dispatch Specialists',
    ],
    photo: 'https://images.unsplash.com/photo-1601584115197-04ecc0da31d7?w=800&q=80&auto=format&fit=crop',
    tone: 'navy',
  },
]

const expandedKey = ref<Industry['key'] | null>(null)

function toggle(key: Industry['key']): void {
  expandedKey.value = expandedKey.value === key ? null : key
}
</script>

<style scoped>
.industries-grid {
  position: relative;
  width: 100%;
  margin-top: var(--panel-overlap);
  padding:
    var(--section-pad-y-lg)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  z-index: 5;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  isolation: isolate;
  font-family: var(--font-family);
}

.industries-grid::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.industries-grid__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

.industries-grid__grid {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(20px, 2.4vw, 32px);
  align-items: start;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

.industries-grid__plus {
  position: relative;
  width: 100%;
  appearance: none;
  -webkit-appearance: none;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: clamp(10px, 1.2vw, 16px);
  min-height: clamp(380px, 38vw, 460px);
  padding: clamp(28px, 3vw, 44px);
  color: #fff;
  font-family: var(--font-family);
  text-decoration: none;
  isolation: isolate;
  overflow: hidden;
  cursor: pointer;
  border: 1px solid rgba(255, 255, 255, 0.10);
  background: linear-gradient(
    135deg,
    rgba(229, 45, 39, 0.92) 0%,
    rgba(229, 45, 39, 0.70) 100%
  );
  border-radius:
    0 clamp(40px, 5.5vw, 72px)
    0 clamp(40px, 5.5vw, 72px);
  transition:
    border-color 0.4s ease,
    transform 0.4s var(--ease-brand),
    box-shadow 0.4s ease;
}

.industries-grid__plus:hover {
  border-color: rgba(255, 255, 255, 0.30);
  transform: translateY(-4px);
  box-shadow: 0 20px 44px rgba(229, 45, 39, 0.32);
}

.industries-grid__plus::before {
  content: '';
  position: absolute;
  inset: 0;
  z-index: 0;
  background: radial-gradient(
    ellipse at center,
    rgba(255, 255, 255, 0.10) 0%,
    transparent 60%
  );
  pointer-events: none;
}

.industries-grid__plus-symbol {
  position: relative;
  z-index: 1;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: clamp(96px, 12vw, 160px);
  font-weight: 200;
  line-height: 1;
  letter-spacing: -0.04em;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.30);
  animation: plus-pulse 4s ease-in-out infinite;
}

@keyframes plus-pulse {
  0%, 100% { transform: scale(1); }
  50%      { transform: scale(1.04); }
}

@media (prefers-reduced-motion: reduce) {
  .industries-grid__plus-symbol { animation: none; }
}

.industries-grid__plus-label {
  position: relative;
  z-index: 1;
  font-size: clamp(15px, 1.4vw, 18px);
  font-weight: 600;
  text-align: center;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

.industries-grid__plus-cta {
  position: relative;
  z-index: 1;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: clamp(9px, 1vw, 11px) clamp(20px, 2vw, 26px);
  background: rgba(255, 255, 255, 0.16);
  border: 1px solid rgba(255, 255, 255, 0.55);
  border-radius: 999px;
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 700;
  letter-spacing: 0.04em;
  transition: background 0.25s ease, color 0.25s ease;
}

.industries-grid__plus:hover .industries-grid__plus-cta {
  background: #fff;
  color: var(--c-brand-red);
}

@media (max-width: 1023px) {
  .industries-grid__grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 599px) {
  .industries-grid__grid { grid-template-columns: 1fr; }
}
</style>
