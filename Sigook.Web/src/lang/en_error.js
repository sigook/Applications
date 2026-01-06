/* eslint-disable no-unused-vars */
import { formatFileSize, isDefinedGlobally } from './utils';

const messages = {
    _default: (field) => `This field value is not valid.`,
    after: (field, [target, inclusion]) => `This field must be after ${inclusion ? 'or equal to ' : ''}${target}.`,
    alpha_dash: (field) => `This field may contain alpha-numeric characters as well as dashes and underscores.`,
    alpha_num: (field) => `This field may only contain alpha-numeric characters.`,
    alpha_spaces: (field) => `This field may only contain alphabetic characters as well as spaces.`,
    alpha: (field) => `This field may only contain alphabetic characters.`,
    before: (field, [target, inclusion]) => `This field must be before ${inclusion ? 'or equal to ' : ''}${target}.`,
    between: (field, [min, max]) => `This field must be between ${min} and ${max}.`,
    confirmed: (field) => `This field confirmation does not match.`,
    credit_card: (field) => `This field is invalid.`,
    date_between: (field, [min, max]) => `This field must be between ${min} and ${max}.`,
    date_format: (field, [format]) => `This field must be in the format ${format}.`,
    decimal: (field, [decimals = '*'] = []) => `This field must be numeric and may contain ${!decimals || decimals === '*' ? '' : decimals} decimal points.`,
    digits: (field, [length]) => `This field must be numeric and exactly contain ${length} digits.`,
    dimensions: (field, [width, height]) => `This field must be ${width} pixels by ${height} pixels.`,
    email: (field) => `This field must be a valid email.`,
    ext: (field,extensions) => `This field must be a valid file (${extensions}).`,
    image: (field) => `This field must be an image.`,
    included: (field) => `This field must be a valid value.`,
    integer: (field) => `This field must be an integer.`,
    ip: (field) => `This field must be a valid ip address.`,
    length: (field, [length, max]) => {
        if (max) {
            return `This field length must be between ${length} and ${max}.`;
        }

        return `This field length must be ${length}.`;
    },
    max: (field, [length]) => `This field may not be greater than ${length} characters.`,
    max_value: (field, [max]) => `This field must be ${max} or less.`,
    mimes: (field) => `This field must have a valid file type.`,
    min: (field, [length]) => `This field must be at least ${length} characters.`,
    min_value: (field, [min]) => `This field must be ${min} or more.`,
    excluded: (field) => `This field must be a valid value.`,
    numeric: (field) => `This field may only contain numeric characters.`,
    regex: (field) => `This field format is invalid.`,
    required: (field) => `This field is required.`,
    size: (field, [size]) => `This field size must be less than ${formatFileSize(size)}.`,
    url: (field) => `This field is not a valid URL please use something like www.example.com"`
};

const locale = {
    name: 'en',
    messages,
    attributes: {}
};

if (isDefinedGlobally()) {
    // eslint-disable-next-line
    VeeValidate.Validator.localize({ [locale.name]: locale });
}

export default locale;