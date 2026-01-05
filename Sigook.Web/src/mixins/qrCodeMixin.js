import toastMixin from "./toastMixin";
import downloadFileMixin from "@/mixins/downloadFileMixin";

export default {
    data() {
        return {
            QrImage: null,
            QrImageBlob: null
        };
    },
    mixins: [toastMixin, downloadFileMixin],
    methods: {
        getQRCode(workerCardId) {
            this.isLoading = true;
            this.$store.dispatch("getQRCode", workerCardId)
                .then(response => {
                    this.isLoading = false;
                    this.createImage(response);
                })
                .catch(error => {
                    this.isLoading = false;
                    this.showAlertError(error);
                });
        },
        createImage(blob) {
            let vm = this;
            this.isLoading = true;
            this.QrImageBlob = blob;
            this.QrImage = URL.createObjectURL(blob);
            setTimeout(function () {
                vm.isLoading = false;
                vm.printWorkerId();
            }, 1000);
        },
        printWorkerId() {
            let workerFullName = `${this.worker.firstName} ${this.worker.lastName}`;
            try {
                let elem = document.getElementById("qr-card");
                let myWindow = window.open('', 'PRINT', 'height=600,width=1000');
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

                myWindow.document.close(); // necessary for IE >= 10
                myWindow.focus(); // necessary for IE >= 10*/

                myWindow.print();
                myWindow.close();
                return true;
            } catch (e) {
                this.downloadFileGeneric(this.QrImageBlob, `${workerFullName}.jpeg`);
                return true;
            }
        }
    }
};