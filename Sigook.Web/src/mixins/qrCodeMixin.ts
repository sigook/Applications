import toastMixin from "./toastMixin";
import downloadFileMixin from "./downloadFileMixin";
import { fetchQRCode } from '@/api/requestApi';

export default {
  data() {
    return {
      QrImage: null as string | null,
      QrImageBlob: null as Blob | null
    };
  },
  mixins: [toastMixin, downloadFileMixin],
  methods: {
    getQRCode(workerCardId: string): void {
      (this as any).isLoading = true;
      fetchQRCode(workerCardId)
        .then((response: Blob) => {
          (this as any).isLoading = false;
          (this as any).createImage(response);
        })
        .catch((error: any) => {
          (this as any).isLoading = false;
          (this as any).showAlertError(error);
        });
    },
    createImage(blob: Blob): void {
      (this as any).isLoading = true;
      (this as any).QrImageBlob = blob;
      (this as any).QrImage = URL.createObjectURL(blob);
      setTimeout(() => {
        (this as any).isLoading = false;
        (this as any).printWorkerId();
      }, 1000);
    },
    printWorkerId(): boolean {
      const worker = (this as any).worker;
      const workerFullName = `${worker.firstName} ${worker.lastName}`;
      try {
        const elem = document.getElementById("qr-card");
        const myWindow = window.open('', 'PRINT', 'height=600,width=1000');
        if (!myWindow || !elem) return false;
        myWindow.document.write('<html><head>' +
          '<style>' +
          'body { ' +
          '   font-family: Helvetica, Arial, ' +
          '   sans-serif; ' +
          '   text-align:center;' +
          '   padding-top: 50px;}' +
          'section {' +
          '    height: 330px;\n' +
          '    width: 202px;\n' +
          '    border: 1px dashed #7b7b7b;\n' +
          '    display: block;\n' +
          '    margin: 0 auto;\n' +
          '    padding: 30px 15px;\n' +
          '    box-sizing: border-box;}' +
          'h2 {font-size: 16px;\n' +
          '    border-top: 1px solid rgb(29, 29, 29);\n' +
          '    border-bottom: 1px solid rgb(29, 29, 29);\n' +
          '    padding: 5px;\n' +
          '    text-align: center;\n' +
          '    margin: 0;}' +
          'img {width: 100%;\n' +
          '    margin: 10px auto;\n' +
          '    display: block;}' +
          'p{ opacity: 0.8;\n' +
          '    font-size: 12px;\n' +
          '    line-height: 1.4;\n' +
          '    padding: 0 5px;}' +
          '</style>' +
          '<title>' + workerFullName + '</title>' +
          '');
        myWindow.document.write('</head><body >');
        myWindow.document.write(elem.innerHTML);
        myWindow.document.write('</body></html>');

        myWindow.document.close();
        myWindow.focus();

        myWindow.print();
        myWindow.close();
        return true;
      } catch (e) {
        (this as any).downloadFileGeneric((this as any).QrImageBlob, `${workerFullName}.jpeg`);
        return true;
      }
    }
  }
};
