<template>
    <div>
        <h2 class="bg-dark">{{ $t("MoveAndScale")}}</h2>
        <vue-cropper
                ref='cropper'
                :guides="true"
                :view-mode="2"
                drag-mode="crop"
                :auto-crop-area=".8"
                :min-container-width="250"
                :min-container-height="250"
                :background="true"
                :rotatable="true"
                :src="image"
                alt="Source Image"
                :img-style="{ 'width': '400px', 'height': '400px' }">

        </vue-cropper>

        <div class="options">
            <div class="rotate">
                <button @click="rotate(90)" type="button" class="rotate-left"></button>
                <button @click="rotate(-90)" type="button" class="rotate-right"></button>
            </div>

            <div class="actions">
                <button @click="closeModal" type="button" class="background-btn gray-light-button sm-btn">{{ $t("Cancel")}}</button>
                <button @click="uploadCroppedImage()" type="button" class="background-btn primary-button sm-btn">{{ $t("CropAndUpload")}}</button>
            </div>

        </div>

    </div>
</template>
<script>
    export default {
        props: ['image'],
        data(){
          return {
              imgSrc: null,
              cropImage: null
          }
        },
        components: {
            VueCropper: () => import("vue-cropperjs")
        },
        methods: {
            uploadCroppedImage() {
                this.$refs.cropper.getCroppedCanvas().toBlob((blob) => {
                    let extension = blob.type.split("/");
                    extension = extension[1];

                    let file = new File([blob], "uploaded_file." + extension, { type: blob.type, lastModified: Date.now() });
                    this.$emit('onCrop', file);

                });
            },
            rotate(rotationAngle) {
                this.$refs.cropper.rotate(rotationAngle);
            },
            closeModal(){
                this.$emit('closeModal', file);
            },



        }
    }
</script>
<style lang="scss">

    .options {
        padding:15px 15px 10px;
        display: flex;
        justify-content: space-between;
        .rotate {
            button {
                padding: 8px;
                margin-right:3px;
                width: 30px;
                height: 30px;
                -webkit-border-radius: 3px;
                -moz-border-radius: 3px;
                border-radius: 3px;
                background: no-repeat 50% 50%;
                background-size: 20px;
            }

            .rotate-right {
                background-image: url('../assets/images/rotate-to-right.png');
            }
            .rotate-left {
                background-image: url('../assets/images/rotate-to-left.png');
            }
        }

        .actions {
            .gray-light-button {
                border: 1px solid #b5b4b4;
            }
            button {
                margin-left: 5px;
            }
        }
        
        @media(max-width: 767px){
            .actions {
                display: flex;}
        }

    }

</style>
