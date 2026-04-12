export interface PhoneMask {
  delimiters: string[];
  blocks: number[];
  numericOnly: boolean;
}

export const phoneMask: PhoneMask = {
  delimiters: [' ', '-'],
  blocks: [3, 3, 4],
  numericOnly: true,
};
