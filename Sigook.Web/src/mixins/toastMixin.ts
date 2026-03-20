import Vue from "vue";
import Toast from "../components/ToastContainer.vue";
import ToastModal from "../components/ModalContainer.vue";

const ComponentClass = Vue.extend(Toast);
const ComponentModal = Vue.extend(ToastModal);

interface ToastOptions {
  type: string;
  title: string;
  message: string;
  timer: number;
  customClass: string;
}

interface ModalOptions {
  type: string;
  title: string;
  message: string;
  timer?: number;
  customClass: string;
  confirmButton?: boolean;
  cancelButton?: boolean;
  confirmButtonText?: string;
  cancelButtonText?: string;
}

export default {
  data() {
    return {
      instance: null as any,
      timeOut: null as ReturnType<typeof setTimeout> | null,
    };
  },
  methods: {
    async getErrorMessage(errorMessage: any): Promise<string> {
      if (
        typeof errorMessage === "object" &&
        (errorMessage.data !== null && errorMessage.data !== undefined)
      ) {
        if (errorMessage.data instanceof Blob) {
          errorMessage = await errorMessage.data.text();
          try {
            const parsed = JSON.parse(errorMessage);
            return Object.values(parsed).flat().join(', ');
          } catch {
            return errorMessage;
          }
        }
        errorMessage = errorMessage.data;
      }
      if (typeof errorMessage === "object") {
        const objectMessage = errorMessage;
        errorMessage = "";
        for (const item in objectMessage) {
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
    async showAlertError(errorMessage: any): Promise<void> {
      errorMessage = await (this as any).getErrorMessage(errorMessage);
      if (errorMessage == null || errorMessage === "") return;
      (this as any).createInstance({
        type: "error",
        title: "Warning",
        message: errorMessage,
        timer: 10000,
        customClass: "toast-error",
      });
    },
    showAlertSuccess(successMessage: string): void {
      (this as any).createInstance({
        type: "success",
        title: "Success",
        message: successMessage,
        timer: 4000,
        customClass: "toast-success",
      });
    },
    createInstance({ type, title, message, timer, customClass }: ToastOptions): void {
      const vm = this as any;
      vm.closeModal();

      clearTimeout(vm.timeOut);

      if (timer > 0) {
        vm.timeOut = setTimeout(() => {
          vm.closeModal();
        }, timer);
      }

      vm.instance = new ComponentClass({
        propsData: {
          type,
          title,
          message,
          customClass: ["custom-toast", customClass],
        },
      });

      vm.instance.$mount();
      document.getElementById("app")?.appendChild(vm.instance.$el);
    },
    closeModal(): void {
      if (document.getElementById("toast")) {
        const body = document.getElementsByTagName("body");
        body[0].className = "";
        const toast = document.getElementById("toast");
        if (toast) document.getElementById("app")?.removeChild(toast);
      }
    },
    showAlertConfirm(title: string, message: string, confirmBtnText?: string): Promise<boolean> {
      const vm = this as any;
      const body = document.getElementsByTagName("body");
      confirmBtnText = confirmBtnText ? confirmBtnText : vm.$t("Ok");
      return new Promise((resolve) => {
        vm.createModalInstance({
          type: "warning",
          title,
          message,
          customClass: "toast-warning big-modal",
          confirmButton: true,
          cancelButton: true,
          confirmButtonText: confirmBtnText,
          cancelButtonText: vm.$t("Cancel"),
        });

        vm.instance.$on("response", (response: boolean) => {
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
      customClass,
      confirmButtonText,
      cancelButtonText,
    }: ModalOptions): void {
      const vm = this as any;
      vm.closeModal();

      const body = document.getElementsByTagName("body");
      body[0].className = "alert-open";

      vm.instance = new ComponentModal({
        propsData: {
          type,
          title,
          message,
          customClass: ["custom-toast", customClass],
          confirmButtonText,
          cancelButtonText,
        },
      });

      vm.instance.$mount();
      document.getElementById("app")?.appendChild(vm.instance.$el);
    },
    closeToastModal(): void {
      const body = document.getElementsByTagName("body");
      body[0].className = "";
      if (document.getElementById("toast-modal")) {
        const toast = document.getElementById("toast-modal");
        if (toast) document.getElementById("app")?.removeChild(toast);
      }
    },
  },
  components: {
    Toast,
  },
};
