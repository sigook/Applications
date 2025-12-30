import ImageCompressor from 'image-compressor.js';

export default {
    methods:{
        compressFile(file){
            return new Promise((resolve, reject) => {
                new ImageCompressor(file, {
                    quality: 0.6,
                    success(result) {
                        resolve(result);
                    },
                    error(e) {
                        reject(e.message);
                    },
                });
            });

        }
    }
};