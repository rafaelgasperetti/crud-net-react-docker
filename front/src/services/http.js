import axios from 'axios';
import { getItemSession } from '../utils';

export const http = axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    headers: {
        'Content-Type': 'application/json'
    }
})

export const isExpired = (ret) => {
    return (ret.Codigo == 500 && ret.CodigoInterno == 4)
}

export const HTTP_TOKEN = getItemSession('_token');
export const REJECT_MSG = 'Sessao expirou';