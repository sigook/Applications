<template>
  <div class="global-bg" aria-hidden="true">
    <span class="global-bg__deco glow-cyan-lg"></span>
    <span class="global-bg__deco glow-navy-lg"></span>

    <span class="global-bg__deco ring-lg-white"></span>
    <span class="global-bg__deco ring-md-cyan"></span>

    <span class="global-bg__deco dot-cyan-a"></span>
    <span class="global-bg__deco dot-red-a"></span>
    <span class="global-bg__deco dot-cyan-b"></span>
    <span class="global-bg__deco dot-red-b"></span>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.global-bg {
  position: fixed;
  inset: 0;
  z-index: 0;
  overflow: hidden;
  pointer-events: none;
  background:
    radial-gradient(circle at 30% 45%, rgba(255, 255, 255, 0.10) 0%, transparent 36%),
    radial-gradient(circle at 85% 55%, rgba(255, 255, 255, 0.11) 0%, transparent 38%),
    radial-gradient(circle at 35% 82%, rgba(255, 255, 255, 0.08) 0%, transparent 32%),
    radial-gradient(circle at 95% 88%, rgba(255, 255, 255, 0.09) 0%, transparent 34%),
    radial-gradient(circle at 80% 20%, rgba(0, 173, 239, 0.15) 0%, transparent 45%),
    linear-gradient(
      180deg,
      var(--c-brand-navy) 0%,
      #093055 22%,
      #0d4063 48%,
      #135b8c 76%,
      #1575bb 100%
    );
}

.global-bg__deco {
  position: absolute;
  border-radius: 50%;
  will-change: transform;
}
.glow-cyan-lg {
  width: 640px; height: 640px;
  top: -100px; right: -100px;
  background: #fff;
  filter: blur(180px);
  opacity: 0.18;
  animation: drift-a 62s ease-in-out infinite;
}

.glow-navy-lg {
  width: 520px; height: 520px;
  bottom: -120px; left: -120px;
  background: #fff;
  filter: blur(160px);
  opacity: 0.14;
  animation: drift-b 70s ease-in-out infinite;
  animation-delay: -8s;
}

.ring-lg-white {
  width: 220px; height: 220px;
  top: 12%; left: 8%;
  background: transparent;
  border: 3px solid var(--c-glass-border);
  animation: drift-g 54s ease-in-out infinite;
}

.ring-md-cyan {
  width: 130px; height: 130px;
  bottom: 22%; right: 8%;
  background: transparent;
  border: 3px solid rgba(0, 173, 239, 0.55);
  animation: drift-h 46s ease-in-out infinite;
  animation-delay: -12s;
}

.dot-cyan-a {
  width: 22px; height: 22px;
  top: 28%; right: 8%;
  background: var(--c-brand-cyan);
  opacity: 0.85;
  animation: drift-c 38s ease-in-out infinite;
}

.dot-red-a {
  width: 14px; height: 14px;
  bottom: 24%; left: 18%;
  background: var(--c-brand-red);
  opacity: 0.85;
  animation: drift-a 42s ease-in-out infinite;
  animation-delay: -18s;
}

.dot-cyan-b {
  width: 12px; height: 12px;
  top: 62%; left: 6%;
  background: var(--c-brand-cyan);
  opacity: 0.70;
  animation: drift-e 44s ease-in-out infinite;
  animation-delay: -10s;
}

.dot-red-b {
  width: 18px; height: 18px;
  top: 18%; left: 48%;
  background: var(--c-brand-red);
  opacity: 0.75;
  animation: drift-b 36s ease-in-out infinite;
  animation-delay: -22s;
}

@keyframes drift-a {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(140px, -110px); }
}

@keyframes drift-b {
  0%, 100% { transform: translate(0, 0); }
  33%      { transform: translate(-90px, 80px); }
  66%      { transform: translate(80px, -120px); }
}

@keyframes drift-c {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(-110px, 90px); }
}

@keyframes drift-e {
  0%, 100% { transform: translate(0, 0); }
  25%      { transform: translate(80px, -60px); }
  50%      { transform: translate(50px, 100px); }
  75%      { transform: translate(-80px, 40px); }
}

@keyframes drift-g {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(-130px, 150px); }
}

@keyframes drift-h {
  0%, 100% { transform: translate(0, 0); }
  33%      { transform: translate(120px, 70px); }
  66%      { transform: translate(-90px, 110px); }
}

@media (prefers-reduced-motion: reduce) {
  .global-bg__deco { animation: none !important; }
}

@media (max-width: 1023px) {
  .global-bg {
    background:
      radial-gradient(circle at 85% 8%, rgba(255, 255, 255, 0.10) 0%, transparent 38%),
      radial-gradient(circle at 10% 90%, rgba(255, 255, 255, 0.07) 0%, transparent 42%),
      radial-gradient(circle at 82% 60%, var(--c-glass-fill) 0%, transparent 40%),
      radial-gradient(circle at 80% 20%, rgba(0, 173, 239, 0.15) 0%, transparent 45%),
      linear-gradient(
        180deg,
        var(--c-brand-navy) 0%,
        #093055 22%,
        #0d4063 48%,
        #135b8c 76%,
        #1575bb 100%
      );
  }

  .global-bg__deco {
    animation: none !important;
    will-change: auto;
  }

  .glow-cyan-lg,
  .glow-navy-lg {
    display: none;
  }

  .ring-lg-white { width: 140px; height: 140px; }
  .ring-md-cyan  { width: 80px;  height: 80px;  }
}
</style>
