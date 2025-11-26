<template>
  <form class="contact-form" @submit.prevent>
    <!-- Encabezado -->
    <header class="contact-form__header">
      <h2 class="contact-form__title">Your Information</h2>
      <p class="contact-form__subtitle">
        We’ll reach out to you for the next steps
      </p>
    </header>

    <!-- Nombre -->
    <div class="contact-form__group contact-form__group--two">
      <div class="contact-form__field">
        <label class="contact-form__label" for="firstName">Name</label>
        <input
          id="firstName"
          type="text"
          class="contact-form__input"
          placeholder="First Name"
        />
      </div>

      <div class="contact-form__field contact-form__field--no-label">
        <label class="contact-form__label" for="lastName">Last Name</label>
        <input
          id="lastName"
          type="text"
          class="contact-form__input"
          placeholder="Last Name"
        />
      </div>
    </div>

    <!-- Email -->
    <div class="contact-form__group">
      <label class="contact-form__label" for="email">Email</label>
      <input
        id="email"
        type="email"
        class="contact-form__input"
        placeholder="Your Email Address"
      />
    </div>

    <!-- Industry -->
    <div class="contact-form__group">
      <label class="contact-form__label" for="industry">Industry</label>
      <select id="industry" class="contact-form__input contact-form__select">
        <option value="" disabled selected>Select One</option>
        <option value="manufacturing">Manufacturing</option>
        <option value="logistics">Logistics</option>
        <option value="construction">Construction</option>
        <option value="other">Other</option>
      </select>
    </div>

    <!-- Message -->
    <div class="contact-form__group">
      <label class="contact-form__label" for="message">Message</label>
      <textarea
        id="message"
        class="contact-form__input contact-form__textarea"
        rows="4"
        placeholder="Tell us more about the role you need to fill"
      ></textarea>
    </div>

    <!-- Botón principal -->
    <button type="submit" class="contact-form__submit">
      Save &amp; Send
    </button>

    <!-- Reset -->
    <button type="button" class="contact-form__reset">
      Reset Information
    </button>
  </form>
</template>

<script setup>
// sin lógica por ahora; luego acá agregas emits / composables para llamar a la API
</script>

<style scoped>
* {
  box-sizing: border-box;
}

/* Contenedor del formulario (usa todo el ancho del card padre) */
.contact-form {
  width: 100%;
}

/* Encabezado */
.contact-form__header {
  text-align: center;
  margin-bottom: 28px;
}

.contact-form__title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #222222;
  margin-bottom: 4px;
}

.contact-form__subtitle {
  font-size: 0.9rem;
  color: #a0a0a0;
}

/* Grupos */
.contact-form__group {
  margin-bottom: 18px;
}

/* Layout dos columnas para nombre */
.contact-form__group--two {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.contact-form__field--no-label .contact-form__label {
  /* usamos el mismo label pero la palabra solo sirve para accesibilidad */
}

/* Labels */
.contact-form__label {
  display: block;
  font-size: 0.9rem;
  font-weight: 600;
  color: #222222;
  margin-bottom: 6px;
}

/* Inputs / selects / textarea */
.contact-form__input {
  width: 100%;
  padding: 10px 16px;
  border-radius: 999px;
  border: 1px solid #ececec;
  background-color: #fafafa;
  font-size: 0.9rem;
  outline: none;
  transition: border-color 0.18s ease, box-shadow 0.18s ease,
    background-color 0.18s ease;
  color: #333333;
}

.contact-form__input::placeholder {
  color: #c3c3c3;
}

/* textareas: esquinas suavemente redondeadas pero más rectangulares */
.contact-form__textarea {
  border-radius: 18px;
  resize: vertical;
  min-height: 120px;
  padding-top: 12px;
}

/* select */
.contact-form__select {
  appearance: none;
  -webkit-appearance: none;
  -moz-appearance: none;
  background-image: linear-gradient(45deg, transparent 50%, #c3c3c3 50%),
    linear-gradient(135deg, #c3c3c3 50%, transparent 50%);
  background-position: calc(100% - 18px) calc(50% - 3px),
    calc(100% - 13px) calc(50% - 3px);
  background-size: 6px 6px, 6px 6px;
  background-repeat: no-repeat;
}

/* focus */
.contact-form__input:focus,
.contact-form__textarea:focus,
.contact-form__select:focus {
  border-color: #32d26a;
  background-color: #ffffff;
  box-shadow: 0 0 0 2px rgba(50, 210, 106, 0.12);
}

/* Botón principal */
.contact-form__submit {
  width: 100%;
  margin-top: 8px;
  padding: 12px 20px;
  border-radius: 999px;
  border: none;
  outline: none;
  background-color: #32d26a;
  color: #ffffff;
  font-weight: 700;
  font-size: 0.95rem;
  cursor: pointer;
  box-shadow: 0 14px 30px rgba(0, 0, 0, 0.25);
  transition: background-color 0.18s ease, transform 0.18s ease,
    box-shadow 0.18s ease;
}

.contact-form__submit:hover {
  background-color: #27b058;
  transform: translateY(-1px);
  box-shadow: 0 18px 36px rgba(0, 0, 0, 0.3);
}

/* Reset */
.contact-form__reset {
  display: block;
  margin: 14px auto 0;
  border: none;
  background: none;
  color: #333333;
  font-size: 0.85rem;
  cursor: pointer;
  text-decoration: none;
}

.contact-form__reset:hover {
  text-decoration: underline;
}

/* Responsive pequeño */
@media (max-width: 480px) {
  .contact-form__group--two {
    grid-template-columns: 1fr;
  }

  .contact-form__title {
    font-size: 1.4rem;
  }
}
</style>
