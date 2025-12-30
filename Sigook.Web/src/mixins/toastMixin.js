import Vue from "vue";
import Toast from "../components/ToastContainer";
import ToastModal from "../components/ModalContainer";

let ComponentClass = Vue.extend(Toast);
let ComponentModal = Vue.extend(ToastModal);

export default {
  data() {
    return {
      instance: null,
      timeOut: null,
    };
  },
  methods: {
    getErrorMessage(errorMessage) {
      if (
        typeof errorMessage === "object" &&
        (errorMessage.data !== null && errorMessage.data !== undefined)
      ) {
        errorMessage = errorMessage.data;
      }
      if (typeof errorMessage === "object") {
        let objectMessage = errorMessage;
        errorMessage = "";
        for (let item in objectMessage) {
          if (
            typeof objectMessage[item] === "object" &&
            objectMessage[item].message
          ) {
            errorMessage += objectMessage[item].message + "<br/>";
          } else {
            errorMessage += objectMessage[item] + "<br/>";
          }
        }
      }
      return errorMessage;
    },
    showAlertError(errorMessage) {
      errorMessage = this.getErrorMessage(errorMessage);
      if (errorMessage == null || errorMessage === "") return;
      this.createInstance({
        type: "error",
        title: "Warning",
        message: errorMessage,
        timer: 10000,
        customClass: "toast-error",
      });
    },
    showAlertSuccess(successMessage) {
      this.createInstance({
        type: "success",
        title: "Success",
        message: successMessage,
        timer: 4000,
        customClass: "toast-success",
      });
    },
    createInstance({ type, title, message, timer, customClass }) {
      this.closeModal();

      clearTimeout(this.timeOut);

      if (timer > 0) {
        let vm = this;
        this.timeOut = setTimeout(function() {
          vm.closeModal();
        }, timer);
      }

      this.instance = new ComponentClass({
        propsData: {
          type: type,
          title: title,
          message: message,
          customClass: ["custom-toast", customClass],
        },
      });

      this.instance.$mount();
      document.getElementById("app").appendChild(this.instance.$el);
    },
    closeModal() {
      if (document.getElementById("toast")) {
        let body = document.getElementsByTagName("body");
        body[0].className = "";
        let toast = document.getElementById("toast");
        document.getElementById("app").removeChild(toast);
      }
    },
    showAlertConfirm(title, message, confirmBtnText) {
      let vm = this;
      let body = document.getElementsByTagName("body");
      confirmBtnText = confirmBtnText ? confirmBtnText : vm.$t("Ok");
      return new Promise((resolve, reject) => {
        this.createModalInstance({
          type: "warning",
          title: title,
          message: message,
          customClass: "toast-warning big-modal",
          confirmButton: true,
          cancelButton: true,
          confirmButtonText: confirmBtnText,
          cancelButtonText: vm.$t("Cancel"),
        });

        this.instance.$on("response", function(response) {
          if (response) {
            resolve(response);
            vm.closeToastModal();
          } else {
            resolve(response);
          }
          body[0].className = "";
        });
      });
    },
    createModalInstance({
      type,
      title,
      message,
      timer,
      customClass,
      confirmButtonText,
      cancelButtonText,
    }) {
      this.closeModal();

      let body = document.getElementsByTagName("body");
      body[0].className = "alert-open";

      this.instance = new ComponentModal({
        propsData: {
          type: type,
          title: title,
          message: message,
          customClass: ["custom-toast", customClass],
          confirmButtonText: confirmButtonText,
          cancelButtonText: cancelButtonText,
        },
      });

      this.instance.$mount();
      document.getElementById("app").appendChild(this.instance.$el);
    },
    closeToastModal() {
      let body = document.getElementsByTagName("body");
      body[0].className = "";
      if (document.getElementById("toast-modal")) {
        let toast = document.getElementById("toast-modal");
        document.getElementById("app").removeChild(toast);
      }
    },
  },
  components: {
    Toast,
  },
};
