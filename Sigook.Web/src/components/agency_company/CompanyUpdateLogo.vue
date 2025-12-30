<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <transition name="modal">
      <div class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container update-logo-modal">
              <span class="fz1 fw-700 ">Logo</span>
              <button @click="cancelUpdate" type="button" class="cross-icon">{{ $t('Close') }}</button>

              <upload-image v-if="newLogo" @imageSelected="profileImg => this.newLogo = { fileName: profileImg }"
                :edited-image="this.logo" :required="false" @onUpload="() => subscribe('file')"
                @finishUpload="() => unsubscribe()" class="margin-10-auto">
              </upload-image>
              <div class="text-center">
                <button class="background-btn md-btn red-button btn-radius margin-top-15 margin-right uppercase"
                  @click="cancelUpdate" type="button">{{ $t("Cancel") }}
                </button>
                <button class="background-btn md-btn primary-button btn-radius margin-top-15 uppercase"
                  @click="updateLogo()" type="button">{{ $t("Save") }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import UploadImage from "@/components/PreviewImage";
import pubSub from "@/mixins/pubSub";

export default {
  name: "CompanyUpdateLogo",
  mixins: [pubSub],
  components: { UploadImage },
  props: ["logo"],
  data() {
    return {
      newLogo: {},
      isLoading: false
    }
  },
  methods: {
    updateLogo() {
      if (!this.newLogo || this.logo.fileName === this.newLogo.fileName) {
        this.$emit("cancel");
      } else {
        this.$emit("save", this.newLogo);
      }
    },
    cancelUpdate() {
      this.$emit("cancel");
    }
  },
  created() {
    if (this.logo != null) {
      this.newLogo = Object.assign({}, this.logo);
    }
  }
}
</script>
<style lang="scss">
.update-logo-modal {

  .fz1 {
    margin-bottom: 20px;
    display: inline-block;
    position: relative;
    top: -5px;

  }

  .update-image label {
    z-index: 3;
    color: white;

  }
}
</style>