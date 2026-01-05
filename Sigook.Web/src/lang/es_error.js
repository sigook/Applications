import { formatFileSize, isDefinedGlobally } from './utils';

const messages = {
    _default: (field) => `Este campo no es válido.`,
    after: (field, [target, inclusion]) => `Este campo debe ser posterior ${inclusion ? 'o igual ' : ''}a ${target}.`,
    alpha_dash: (field) => `Este campo solo debe contener letras, números y guiones.`,
    alpha_num: (field) => `Este campo solo debe contener letras y números.`,
    alpha_spaces: (field) => `Este campo solo debe contener letras y espacios.`,
    alpha: (field) => `Este campo solo debe contener letras.`,
    before: (field, [target, inclusion]) => `Este campo debe ser anterior ${inclusion ? 'o igual ' : ''}a ${target}.`,
    between: (field, [min, max]) => `Este campo debe estar entre ${min} y ${max}.`,
    confirmed: (field) => `Este campo no coincide.`,
    credit_card: (field) => `Este campo es inválido.`,
    date_between: (field, [min, max]) => `Este campo debe estar entre ${min} y ${max}.`,
    date_format: (field, [format]) => `Este campo debe tener formato formato ${format}.`,
    decimal: (field, [decimals = '*'] = []) => `Este campo debe ser numérico y contener ${decimals === '*' ? '' : decimals} puntos decimales.`,
    digits: (field, [length]) => `Este campo debe ser numérico y contener exactamente ${length} dígitos.`,
    dimensions: (field, [width, height]) => `Este campo debe ser de ${width} píxeles por ${height} píxeles.`,
    email: (field) => `Este campo debe ser un correo electrónico válido.`,
    ext: (field) => `Este campo debe ser un archivo válido.`,
    image: (field) => `Este campo debe ser una imagen.`,
    included: (field) => `Este campo debe ser un valor válido.`,
    integer: (field) => `Este campo debe ser un entero.`,
    ip: (field) => `Este campo debe ser una dirección ip válida.`,
    length: (field, [length, max]) => {
        if (max) {
            return `El largo de este campo debe estar entre ${length} y ${max}.`;
        }

        return `El largo de este campo debe ser ${length}.`;
    },
    max: (field, [length]) => `Este campo no debe ser mayor a ${length} caracteres.`,
    max_value: (field, [max]) => `Este campo debe de ser ${max} o menor.`,
    mimes: (field) => `Este campo debe ser un tipo de archivo válido.`,
    min: (field, [length]) => `Este campo debe tener al menos ${length} caracteres.`,
    min_value: (field, [min]) => `Este campo debe ser ${min} o superior.`,
    excluded: (field) => `Este campo debe ser un valor válido.`,
    numeric: (field) => `Este campo debe contener solo caracteres numéricos.`,
    regex: (field) => `El formato de este campo no es válido.`,
    required: (field) => `Este campo es obligatorio.`,
    size: (field, [size]) => `Este campo debe ser menor a ${formatFileSize(size)}.`,
    url: (field) => `Este campo no es una URL válida.`
};

const locale = {
    name: 'es',
    messages,
    attributes: {}
};

if (isDefinedGlobally()) {
    // eslint-disable-next-line
    VeeValidate.Validator.localize({ [locale.name]: locale });
}

export default locale;