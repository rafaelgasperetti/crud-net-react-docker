const Swal = require('sweetalert2');

export const disconnect = () => {
    try{
        const get = getItemSession('_dados');
                            
        setTimeout(() => {  
            sessionStorage.clear();
            localStorage.clear();
            window.location.href = "https://localhost:49959/login";
            alert('Oops! A sessão expirou e você será redirecionado(a). \n \nPor favor, efetue o login novamente');
        }, 1000); 

    }catch(e){        
        sessionStorage.clear();
        localStorage.clear();
        window.location.href = '/unauthorized';
        alert('Oops! Ocorreu um erro inesperado e você será redirecionado(a).\n\nPor favor, efetue o login novamente');
    }
}

export const verificationBrowser = () => {
    var nav = navigator.userAgent.toLowerCase();
        
        var browser = '';
        if(nav.indexOf("mozilla") != -1){
            if(nav.indexOf("firefox") != -1){
                browser = "firefox";
            }else if(nav.indexOf("firefox") != -1){
                browser = "mozilla";
            }else if(nav.indexOf("chrome") != -1){
                browser = "chrome";
            }            
        }
    return browser;
}

export const formatMoney = (amount, decimalCount = 2, decimal = ".", thousands = ",") => {
    try {
        decimalCount = Math.abs(decimalCount);
        decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

        const negativeSign = amount < 0 ? "-" : "";

        let i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
        let j = (i.length > 3) ? i.length % 3 : 0;

        return negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
    } catch (e) {
        console.log(e)
    }
};

// export const formatMoney = (amount) => {
//     let formato = { minimumFractionDigits: 2 , style: 'currency', currency: 'BRL' };
//     return amount.toLocaleString('pt-BR', formato);
// }

export const formatDateToView = (inputDate) => {
    let date = new Date(inputDate);
    return date.toLocaleDateString('pt-BR', { timeZone: 'UTC' });
}

export const formatDateToDatabase = (inputDate) => {
    return inputDate.split("/").reverse().join("-");
}

export const ucfirst = (str) => {
    try {
        var text = str.toLowerCase();

        var parts = text.split(' '),
            len = parts.length,
            i, words = [];

        for (i = 0; i < len; i++) {
            var part = parts[i];
            var first = part[0].toUpperCase();
            var rest = part.substring(1, part.length);
            var word = first + rest;
            words.push(word);
        }

        return words.join(' ');
    } catch (e) {
        console.log(e)
    }
};

export const filtersLimitWord = (term) => {
    const limit = 20;
    if (term.length > limit) {
        return term.substring(0, limit) + '...';
    }
    return term;
}

// uso: formatCNPJ(12123123123412)
// retorna: 12.123.123/1234-12
export const formatCNPJ = (cnpj) => {
    const padCnpj = padDigits(cnpj, 14);
    if (padCnpj.length === 14) {
        return padCnpj.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5")
    }
    return padCnpj;
}

// funcao auxiliar para a formatacao do cnpj
export const padDigits = (number, digits) => {
    return Array(Math.max(digits - String(number).length + 1, 0)).join(0) + number;
}

/**
 * https://ourcodeworld.com/articles/read/764/how-to-sort-alphabetically-an-array-of-objects-by-key-in-javascript
 * Function to sort alphabetically an array of objects by some specific key.
 * 
 * @param {String} property Key of the object to sort.
 * MyData.sort(dynamicSort("name")); // name asc
 * MyData.sort(dynamicSort("-name")); // name desc
 */
export const dynamicSort = (property) => {
    var sortOrder = 1;

    if (property[0] === "-") {
        sortOrder = -1;
        property = property.substr(1);
    }

    return function (a, b) {
        if (sortOrder === -1) {
            return b[property].localeCompare(a[property]);
        } else {
            return a[property].localeCompare(b[property]);
        }
    }
}

export const scrollToTop = () => {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Entra ["1", "2", "3", "4", "5"]
// Sai [1, 2, 3, 4, 5]
export const arrayStringToArrayInt = (arrayString) => {
    var temp = new Array();
    temp = arrayString.split(",");
    for (let a in temp) {
        temp[a] = parseInt(temp[a], 10);
    }

    return filterArray(temp)
}

function filterArray(test_array) {
    let index = -1;
    const arr_length = test_array ? test_array.length : 0;
    let resIndex = -1;
    const result = [];

    while (++index < arr_length) {
        const value = test_array[index];

        if (value) {
            result[++resIndex] = value;
        }
    }

    return result;
}

export const setItemSession = (nome, dado) => {
    sessionStorage.setItem(nome, dado);
}

export const getItemSession = (nome) =>{
    return sessionStorage.getItem(nome);
}