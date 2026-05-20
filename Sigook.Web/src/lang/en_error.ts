import { formatFileSize, isDefinedGlobally } from './utils';

const messages = {
    _default: (_field) => `This field value is not valid.`,
    after: (_field, [target, inclusion]) => `This field must be after ${inclusion ? 'or equal to ' : ''}${target}.`,
    alpha_dash: (_field) => `This field may contain alpha-numeric characters as well as dashes and underscores.`,
    alpha_num: (_field) => `This field may only contain alpha-numeric characters.`,
    alpha_spaces: (_field) => `This field may only contain alphabetic characters as well as spaces.`,
    alpha: (_field) => `This field may only contain alphabetic characters.`,
    before: (_field, [target, inclusion]) => `This field must be before ${inclusion ? 'or equal to ' : ''}${target}.`,
    between: (_field, [min, max]) => `This field must be between ${min} and ${max}.`,
    confirmed: (_field) => `This field confirmation does not match.`,
    credit_card: (_field) => `This field is invalid.`,
    date_between: (_field, [min, max]) => `This field must be between ${min} and ${max}.`,
    date_format: (_field, [format]) => `This field must be in the format ${format}.`,
    decimal: (_field, [decimals = '*'] = []) => `This field must be numeric and may contain ${!decimals || decimals === '*' ? '' : decimals} decimal points.`,
    digits: (_field, [length]) => `This field must be numeric and exactly contain ${length} digits.`,
    dimensions: (_field, [width, height]) => `This field must be ${width} pixels by ${height} pixels.`,
    email: (_field) => `This field must be a valid email.`,
    ext: (_field,extensions) => `This field must be a valid file (${extensions}).`,
    image: (_field) => `This field must be an image.`,
    included: (_field) => `This field must be a valid value.`,
    integer: (_field) => `This field must be an integer.`,
    ip: (_field) => `This field must be a valid ip address.`,
    length: (_field, [length, max]) => {
        if (max) {
            return `This field length must be between ${length} and ${max}.`;
        }

        return `This field length must be ${length}.`;
    },
    max: (_field, [length]) => `This field may not be greater than ${length} characters.`,
    max_value: (_field, [max]) => `This field must be ${max} or less.`,
    mimes: (_field) => `This field must have a valid file type.`,
    min: (_field, [length]) => `This field must be at least ${length} characters.`,
    min_value: (_field, [min]) => `This field must be ${min} or more.`,
    excluded: (_field) => `This field must be a valid value.`,
    numeric: (_field) => `This field may only contain numeric characters.`,
    regex: (_field) => `This field format is invalid.`,
    required: (_field) => `This field is required.`,
    size: (_field, [size]) => `This field size must be less than ${formatFileSize(size)}.`,
    url: (_field) => `This field is not a valid URL please use something like www.example.com"`
};

const locale = {
    name: 'en',
    messages,
    attributes: {}
};

if (isDefinedGlobally()) {
    (window as any).VeeValidate.Validator.localize({ [locale.name]: locale });
}

export default locale;