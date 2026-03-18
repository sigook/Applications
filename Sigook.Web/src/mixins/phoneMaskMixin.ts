interface PhoneMask {
  delimiters: string[];
  blocks: number[];
  numericOnly: boolean;
}

export default {
  data() {
    return {
      mask: {
        delimiters: [' ', '-'],
        blocks: [3, 3, 4],
        numericOnly: true,
      } as PhoneMask
    };
  }
};
