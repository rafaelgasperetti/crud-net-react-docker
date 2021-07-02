import { http, isExpired, HTTP_TOKEN, REJECT_MSG } from './http';
import { arrayStringToArrayInt } from '../utils';

//
export const obterProdutos = (id, nome, descricao) => {
    const options = {
        "Token": HTTP_TOKEN,
        "Esquema": "dbo",
        "Tabela": "Produto",
        "Parametros": [
        ]
    }

    if(id) {
        options.Parametros.push({
            "Nome": "Id",
            "Valor": parseInt(id)
        });
    }

    if(nome) {
        options.Parametros.push({
            "Nome": "Nome",
            "Valor": nome
        });
    }

    if(descricao) {
        options.Parametros.push({
            "Nome": "Descricao",
            "Valor": descricao
        });
    }

    return new Promise((resolve, reject) => {
        const ret = http.post(`Dados/ObterDados`, options)

        if (isExpired(ret)) {
            reject(REJECT_MSG);
        }

        resolve(ret);
    })
}

export const obterContagemProdutos = () => {
    const options = {
        "Token": HTTP_TOKEN,
        "Esquema": "dbo",
        "Tabela": "Produto",
        "Parametros": [
        ]
    }

    return new Promise((resolve, reject) => {
        const ret = http.post(`Dados/ObterContagemDados`, options)

        if (isExpired(ret)) {
            reject(REJECT_MSG);
        }

        resolve(ret);
    })
}

//
export const enviarProdutos = (operacao, produto) => {
    const options = {
        "Token": HTTP_TOKEN,
        "Esquema": "dbo",
        "Tabela": "Produto",
        "Operacao": parseInt(operacao),
        "Data": produto
    }

    return new Promise((resolve, reject) => {
        const ret = http.post(`Dados/EnviarDados`, options)

        if (isExpired(ret)) {
            reject(REJECT_MSG);
        }

        resolve(ret);
    })
}