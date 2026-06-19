<template>
  <div class="ttp-detail">
    <section class="ttp-detail__block">
      <h4 class="ttp-detail__heading">How it works</h4>
      <ol class="ttp-detail__steps">
        <li>We match you with a vetted candidate on a temporary contract</li>
        <li>They work on-site, proving their skills and fit with your team</li>
        <li>Keep them on contract as long as you need — or convert to permanent whenever you're ready</li>
      </ol>
    </section>

    <section class="ttp-detail__block">
      <h4 class="ttp-detail__heading">Why employers choose this path</h4>
      <ul class="ttp-detail__list">
        <li>Flexibility to scale your team up or down without long-term commitment</li>
        <li>See real, on-the-job performance before making a permanent decision</li>
        <li>Avoid the cost and risk of a bad permanent hire</li>
        <li>Convert on your timeline — 30 days, 6 months, or stay temp indefinitely</li>
      </ul>
    </section>

    <section class="ttp-detail__block">
      <h4 class="ttp-detail__heading">What Sigook handles for you</h4>
      <ul class="ttp-detail__list">
        <li>Payroll, taxes, and compliance for the full duration of the contract</li>
        <li>A simple, fast conversion process whenever you decide to hire permanent</li>
        <li>A dedicated recruiter supporting the engagement from day one</li>
      </ul>
    </section>

    <p class="ttp-detail__result">
      <span class="ttp-detail__result-label">The result:</span>
      Workforce flexibility today, with the option of a confident permanent hire tomorrow.
    </p>
  </div>
</template>

<script setup lang="ts">
/**
 * TempToPermDetail — the expanded detail revealed inside the Temp-to-Perm
 * SecondaryCard on both the Talents and Employers Solutions sections.
 * Same copy in both places, so it lives here once.
 */
</script>

<style scoped>
.ttp-detail {
  display: flex;
  flex-direction: column;
  gap: clamp(14px, 1.8vw, 20px);
  text-align: left;
}

.ttp-detail__heading {
  font-size: clamp(11px, 1vw, 12px);
  font-weight: 700;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.92);
  margin: 0 0 clamp(8px, 1vw, 10px);
}

.ttp-detail__steps,
.ttp-detail__list {
  margin: 0;
  padding: 0;
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

/* Numbered steps — circular index badge */
.ttp-detail__steps {
  counter-reset: ttp-step;
}

.ttp-detail__steps li {
  counter-increment: ttp-step;
  position: relative;
  padding-left: 28px;
  font-size: clamp(13px, 1.1vw, 14px);
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.90);
}

.ttp-detail__steps li::before {
  content: counter(ttp-step);
  position: absolute;
  left: 0;
  top: 0.05em;
  width: 19px;
  height: 19px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: 700;
  line-height: 1;
  color: #fff;
  background: rgba(255, 255, 255, 0.18);
  border: 1px solid rgba(255, 255, 255, 0.40);
  border-radius: 50%;
}

/* Bulleted lists */
.ttp-detail__list li {
  position: relative;
  padding-left: 18px;
  font-size: clamp(13px, 1.1vw, 14px);
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.88);
}

.ttp-detail__list li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.55em;
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.75);
}

.ttp-detail__result {
  margin: 0;
  font-size: clamp(13px, 1.15vw, 15px);
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.92);
}

.ttp-detail__result-label {
  font-weight: 700;
}
</style>
