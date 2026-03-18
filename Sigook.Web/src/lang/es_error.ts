import { formatFileSize, isDefinedGlobally } from './utils';

const messages = {
    _default: (_field) => `Este campo no es válido.`,
    after: (_field, [target, inclusion]) => `Este campo debe ser posterior ${inclusion ? 'o igual ' : ''}a ${target}.`,
    alpha_dash: (_field) => `Este campo solo debe contener letras, números y guiones.`,
    alpha_num: (_field) => `Este campo solo debe contener letras y números.`,
    alpha_spaces: (_field) => `Este campo solo debe contener letras y espacios.`,
    alpha: (_field) => `Este campo solo debe contener letras.`,
    before: (_field, [target, inclusion]) => `Este campo debe ser anterior ${inclusion ? 'o igual ' : ''}a ${target}.`,
    between: (_field, [min, max]) => `Este campo debe estar entre ${min} y ${max}.`,
    confirmed: (_field) => `Este campo no coincide.`,
    credit_card: (_field) => `Este campo es inválido.`,
    date_between: (_field, [min, max]) => `Este campo debe estar entre ${min} y ${max}.`,
    date_format: (_field, [format]) => `Este campo debe tener formato formato ${format}.`,
    decimal: (_field, [decimals = '*'] = []) => `Este campo debe ser numérico y contener ${decimals === '*' ? '' : decimals} puntos decimales.`,
    digits: (_field, [length]) => `Este campo debe ser numérico y contener exactamente ${length} dígitos.`,
    dimensions: (_field, [width, height]) => `Este campo debe ser de ${width} píxeles por ${height} píxeles.`,
    email: (_field) => `Este campo debe ser un correo electrónico válido.`,
    ext: (_field) => `Este campo debe ser un archivo válido.`,
    image: (_field) => `Este campo debe ser una imagen.`,
    included: (_field) => `Este campo debe ser un valor válido.`,
    integer: (_field) => `Este campo debe ser un entero.`,
    ip: (_field) => `Este campo debe ser una dirección ip válida.`,
    length: (_field, [length, max]) => {
        if (max) {
            return `El largo de este campo debe estar entre ${length} y ${max}.`;
        }

        return `El largo de este campo debe ser ${length}.`;
    },
    max: (_field, [length]) => `Este campo no debe ser mayor a ${length} caracteres.`,
    max_value: (_field, [max]) => `Este campo debe de ser ${max} o menor.`,
    mimes: (_field) => `Este campo debe ser un tipo de archivo válido.`,
    min: (_field, [length]) => `Este campo debe tener al menos ${length} caracteres.`,
    min_value: (_field, [min]) => `Este campo debe ser ${min} o superior.`,
    excluded: (_field) => `Este campo debe ser un valor válido.`,
    numeric: (_field) => `Este campo debe contener solo caracteres numéricos.`,
    regex: (_field) => `El formato de este campo no es válido.`,
    required: (_field) => `Este campo es obligatorio.`,
    size: (_field, [size]) => `Este campo debe ser menor a ${formatFileSize(size)}.`,
    url: (_field) => `Este campo no es una URL válida.`
};

const locale = {
    name: 'es',
    messages,
    attributes: {}
};

if (isDefinedGlobally()) {
    // eslint-disable-next-line
    (window as any).VeeValidate.Validator.localize({ [locale.name]: locale });
}

export default locale;