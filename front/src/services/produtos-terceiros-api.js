import { http, isExpired, HTTP_TOKEN, REJECT_MSG } from './http';
import { arrayStringToArrayInt } from '../utils';

//
export const obterProdutos = () => {
    const options = {
        "Token": HTTP_TOKEN,
        "Esquema": "dbo",
        "Tabela": "Produto",
        "Parametros": [
        ]
    }

    return new Promise((resolve, reject) => {
        const ret = http.post(`Dados/ObterDadosTerceiros`, options)

        if (isExpired(ret)) {
            reject(REJECT_MSG);
        }

        resolve(ret);
    })
}