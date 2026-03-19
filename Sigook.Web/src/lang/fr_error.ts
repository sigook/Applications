import { formatFileSize, isDefinedGlobally } from './utils';

const messages = {
    _default: (_field) => `Ce champ n'est pas valide.`,
    after: (_field, [target]) => `Ce champ doit être postérieur à ${target}.`,
    alpha_dash: (_field) => `Ce champ ne peut contenir que des caractères alpha-numériques, tirets ou soulignés.`,
    alpha_num: (_field) => `Ce champ ne peut contenir que des caractères alpha-numériques.`,
    alpha_spaces: (_field) => `Ce champ ne peut contenir que des lettres ou des espaces.`,
    alpha: (_field) => `Ce champ ne peut contenir que des lettres.`,
    before: (_field, [target]) => `Ce champ doit être antérieur à ${target}.`,
    between: (_field, [min, max]) => `Ce champ doit être compris entre ${min} et ${max}.`,
    confirmed: (_field, [confirmedField]) => `Ce champ ne correspond pas à ${confirmedField}.`,
    credit_card: (_field) => `Ce champ est invalide.`,
    date_between: (_field, [min, max]) => `Ce champ doit être situé entre ${min} et ${max}.`,
    date_format: (_field, [format]) => `Ce champ doit être au format ${format}.`,
    decimal: (_field, [decimals = '*'] = []) => `Ce champ doit être un nombre et peut contenir ${decimals === '*' ? '' : decimals} décimales.`,
    digits: (_field, [length]) => `Ce champ doit être un nombre entier de ${length} chiffres.`,
    dimensions: (_field, [width, height]) => `Ce champ doit avoir une taille de ${width} pixels par ${height} pixels.`,
    email: (_field) => `Ce champ doit être une adresse e-mail valide.`,
    ext: (_field) => `Ce champ doit être un fichier valide.`,
    image: (_field) => `Ce champ doit être une image.`,
    included: (_field) => `Ce champ doit être une valeur valide.`,
    integer: (_field) => `Ce champ doit être un entier.`,
    ip: (_field) => `Ce champ doit être une adresse IP.`,
    length: (_field, [length, max]) => {
        if (max) {
            return `Ce champ doit contenir entre ${length} et ${max} caractères.`;
        }

        return `Ce champ doit contenir ${length} caractères.`;
    },
    max: (_field, [length]) => `Ce champ ne peut pas contenir plus de ${length} caractères.`,
    max_value: (_field, [max]) => `Ce champ doit avoir une valeur de ${max} ou moins.`,
    mimes: (_field) => `Ce champ doit avoir un type MIME valide.`,
    min: (_field, [length]) => `Ce champ doit contenir au minimum ${length} caractères.`,
    min_value: (_field, [min]) => `Ce champ doit avoir une valeur de ${min} ou plus.`,
    excluded: (_field) => `Ce champ doit être une valeur valide.`,
    numeric: (_field) => `Ce champ ne peut contenir que des chiffres.`,
    regex: (_field) => `Ce champ est invalide.`,
    required: (_field) => `Ce champ est obligatoire.`,
    size: (_field, [size]) => `Ce champ doit avoir un poids inférieur à ${formatFileSize(size)}.`,
    url: (_field) => `Ce champ n'est pas une URL valide.`
};

const locale = {
    name: 'fr',
    messages,
    attributes: {}
};

if (isDefinedGlobally()) {
    // eslint-disable-next-line
    (window as any).VeeValidate.Validator.localize({ [locale.name]: locale });
}

export default locale;