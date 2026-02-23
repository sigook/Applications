class Speech {
    constructor() {
        this.onInit();
        this.currentSound = new Audio();

        $("#stopButton").fadeOut(0);

        this.currentSound.addEventListener('play', () => {
            $("#playButton").fadeOut(0);
            $("#stopButton").fadeIn(0);
        });

        ['pause', 'ended'].forEach(event => this.currentSound.addEventListener(event, () => {
            $("#playButton").fadeIn(0);
            $("#stopButton").fadeOut(0);
        }));
    }

    onInit() {
        const languageElement = document.querySelector("[name='Language']");
        languageElement.addEventListener('change', (event) => this.loadVoices(event.target.value));
        this.loadVoices(languageElement.value);
        document.querySelector("#convertForm").addEventListener('submit', function (event) {
            event.preventDefault();
            const form = $(this);
            if (form.valid()) {
                fetch('/Speech', { method: 'post', body: new URLSearchParams(new FormData(event.target)) })
                    .then(res => res.json())
                    .then(res => {
                        Swal.fire({
                            title: 'Done!',
                            icon: 'success',
                            confirmButtonText: 'Ok'
                        });
                        $("#playButton").val(res);
                        $("#saveButton").val(res);
                    });
            }
        });
        document.querySelector("#playButton").addEventListener('click', (event) => {
            const audio = $("#playButton").val();
            if (!audio) {
                Swal.fire({
                    title: 'Missing Requeriments',
                    text: 'There is not audio',
                    icon: 'info',
                    confirmButtonText: 'Ok'
                });
                return;
            }
            this.playAudio(audio)
        });
        document.querySelector("#stopButton").addEventListener('click', (event) => {
            this.currentSound.pause();
        })
        document.querySelector("#saveButton").addEventListener('click', (event) => {
            const audioBase64 = $("#playButton").val();
            if (!audioBase64) {
                Swal.fire({
                    title: 'Missing Requeriments',
                    text: 'There is not audio',
                    icon: 'info',
                    confirmButtonText: 'Ok'
                });
                return;
            }
            const audioName = $("#audioName").val() || "speechAudio";
            const linkSource = `data:audio/wav;base64,${audioBase64}`;
            const downloadLink = document.createElement("a");
            const fileName = `${audioName}.wav`;
            downloadLink.href = linkSource;
            downloadLink.download = fileName;
            downloadLink.click();
            location.reload();
        });
    }

    loadVoices(language) {
        const voiceElement = document.querySelector("[name='Voice']");
        voiceElement.innerHTML = '';
        fetch(`${language}/Voices`)
            .then(res => res.json())
            .then(res => res.forEach((element, index) => voiceElement.add(new Option(element.localName, element.shortName))));
    }

    playAudio(audio) {
        this.currentSound.src = "data:audio/wav;base64," + audio;
        this.currentSound.play();
    }
}