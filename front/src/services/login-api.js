import { http } from './http';

//
export const efetuarLogin = (usuario, senha) => {
    const data = {
        ChaveApi: process.env.REACT_APP_API_KEY,
        Login: usuario,
        Senha: senha
    }

    return new Promise( (resolve, reject) => {
        const ret = http.post('Login/EfetuarLogin', JSON.stringify(data))
        resolve(ret);
    })
}