import { formatFileSize, isDefinedGlobally } from './utils';

const messages = {
    _default: (field) => `Ce champ n'est pas valide.`,
    after: (field, [target]) => `Ce champ doit être postérieur à ${target}.`,
    alpha_dash: (field) => `Ce champ ne peut contenir que des caractères alpha-numériques, tirets ou soulignés.`,
    alpha_num: (field) => `Ce champ ne peut contenir que des caractères alpha-numériques.`,
    alpha_spaces: (field) => `Ce champ ne peut contenir que des lettres ou des espaces.`,
    alpha: (field) => `Ce champ ne peut contenir que des lettres.`,
    before: (field, [target]) => `Ce champ doit être antérieur à ${target}.`,
    between: (field, [min, max]) => `Ce champ doit être compris entre ${min} et ${max}.`,
    confirmed: (field, [confirmedField]) => `Ce champ ne correspond pas à ${confirmedField}.`,
    credit_card: (field) => `Ce champ est invalide.`,
    date_between: (field, [min, max]) => `Ce champ doit être situé entre ${min} et ${max}.`,
    date_format: (field, [format]) => `Ce champ doit être au format ${format}.`,
    decimal: (field, [decimals = '*'] = []) => `Ce champ doit être un nombre et peut contenir ${decimals === '*' ? '' : decimals} décimales.`,
    digits: (field, [length]) => `Ce champ doit être un nombre entier de ${length} chiffres.`,
    dimensions: (field, [width, height]) => `Ce champ doit avoir une taille de ${width} pixels par ${height} pixels.`,
    email: (field) => `Ce champ doit être une adresse e-mail valide.`,
    ext: (field) => `Ce champ doit être un fichier valide.`,
    image: (field) => `Ce champ doit être une image.`,
    included: (field) => `Ce champ doit être une valeur valide.`,
    integer: (field) => `Ce champ doit être un entier.`,
    ip: (field) => `Ce champ doit être une adresse IP.`,
    length: (field, [length, max]) => {
        if (max) {
            return `Ce champ doit contenir entre ${length} et ${max} caractères.`;
        }

        return `Ce champ doit contenir ${length} caractères.`;
    },
    max: (field, [length]) => `Ce champ ne peut pas contenir plus de ${length} caractères.`,
    max_value: (field, [max]) => `Ce champ doit avoir une valeur de ${max} ou moins.`,
    mimes: (field) => `Ce champ doit avoir un type MIME valide.`,
    min: (field, [length]) => `Ce champ doit contenir au minimum ${length} caractères.`,
    min_value: (field, [min]) => `Ce champ doit avoir une valeur de ${min} ou plus.`,
    excluded: (field) => `Ce champ doit être une valeur valide.`,
    numeric: (field) => `Ce champ ne peut contenir que des chiffres.`,
    regex: (field) => `Ce champ est invalide.`,
    required: (field) => `Ce champ est obligatoire.`,
    size: (field, [size]) => `Ce champ doit avoir un poids inférieur à ${formatFileSize(size)}.`,
    url: (field) => `Ce champ n'est pas une URL valide.`
};

const locale = {
    name: 'fr',
    messages,
    attributes: {}
};

if (isDefinedGlobally()) {
    // eslint-disable-next-line
    VeeValidate.Validator.localize({ [locale.name]: locale });
}

export default locale;