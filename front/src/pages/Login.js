import React, { useState, useEffect, useRef } from 'react';
import { efetuarLogin } from '../services/login-api';
import { setItemSession, getItemSession } from '../utils';
import '../scss/login.scss';

const Login = (props) => {
    const [state, setState] = useState({
        usuario: "",
        senha: ""
    })

    const [loading, setLoading] = useState(false);

    const latestProps = useRef(props);

    useEffect(() => {
        latestProps.current = props;
    }, []);

    const handleChange = (e) => {
        const { id, value } = e.target
        setState(prevState => ({
            ...prevState,
            [id]: value
        }))
    }

    const handleSubmitClick = (e) => {
        e.preventDefault();
        setLoading(true);
        const usuario = state.usuario;
        const senha = state.senha;

        efetuarLogin(usuario, senha)
            .then(result => {
                if (result.data.Codigo === 200) {
                    const resultData = result.data.Data.Retorno;
                    const client = {
                        Login: resultData.Login
                    }

                    setItemSession('_dados', JSON.stringify(client));
                    setItemSession('_token', resultData.Token);

                    window.location.href = process.env.REACT_APP_BASE_URL;
                } else {
                    latestProps.current.history.push('/unauthorized');
                };

            })
            .catch(error => {
                latestProps.current.history.push('/unauthorized');
            })
    }


    // se está logado, direciona para Home
    if (getItemSession('_token')) {
        window.location.href = process.env.REACT_APP_BASE_URL;
    }

    
    return (
        <section className="conteudo_interno container" id="painellogin">
            <div>
                <form>
                    <div className="form-group text-left">
                        <label htmlFor="exampleInputEmail1">Usuário</label>
                        <div className="input-group-prepend">
                            <div className="iconlogin">
                                <svg width="1em" height="1em" viewBox="0 0 16 16" className="bi bi-people" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                                    <path fillRule="evenodd" d="M15 14s1 0 1-1-1-4-5-4-5 3-5 4 1 1 1 1h8zm-7.995-.944v-.002.002zM7.022 13h7.956a.274.274 0 0 0 .014-.002l.008-.002c-.002-.264-.167-1.03-.76-1.72C13.688 10.629 12.718 10 11 10c-1.717 0-2.687.63-3.24 1.276-.593.69-.759 1.457-.76 1.72a1.05 1.05 0 0 0 .022.004zm7.973.056v-.002.002zM11 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4zm3-2a3 3 0 1 1-6 0 3 3 0 0 1 6 0zM6.936 9.28a5.88 5.88 0 0 0-1.23-.247A7.35 7.35 0 0 0 5 9c-4 0-5 3-5 4 0 .667.333 1 1 1h4.216A2.238 2.238 0 0 1 5 13c0-1.01.377-2.042 1.09-2.904.243-.294.526-.569.846-.816zM4.92 10c-1.668.02-2.615.64-3.16 1.276C1.163 11.97 1 12.739 1 13h3c0-1.045.323-2.086.92-3zM1.5 5.5a3 3 0 1 1 6 0 3 3 0 0 1-6 0zm3-2a2 2 0 1 0 0 4 2 2 0 0 0 0-4z"/>
                                </svg>
                            </div>
                            
                            <input type="text"
                                className="form-control"
                                id="usuario"
                                aria-describedby="usuarioHelp"
                                placeholder="CNPJ"
                                value={state.usuario}
                                onChange={handleChange}
                                
                            />
                        </div>

                    </div>
                    <div className="form-group text-left">
                        <label htmlFor="exampleInputPassword1">Senha</label>
                        <div className="input-group-prepend">
                            <div className="iconlogin">
                                <svg width="1em" height="1em" viewBox="0 0 16 16" className="bi bi-key" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                                    <path fillRule="evenodd" d="M0 8a4 4 0 0 1 7.465-2H14a.5.5 0 0 1 .354.146l1.5 1.5a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0L13 9.207l-.646.647a.5.5 0 0 1-.708 0L11 9.207l-.646.647a.5.5 0 0 1-.708 0L9 9.207l-.646.647A.5.5 0 0 1 8 10h-.535A4 4 0 0 1 0 8zm4-3a3 3 0 1 0 2.712 4.285A.5.5 0 0 1 7.163 9h.63l.853-.854a.5.5 0 0 1 .708 0l.646.647.646-.647a.5.5 0 0 1 .708 0l.646.647.646-.647a.5.5 0 0 1 .708 0l.646.647.793-.793-1-1h-6.63a.5.5 0 0 1-.451-.285A3 3 0 0 0 4 5z"/>
                                    <path d="M4 8a1 1 0 1 1-2 0 1 1 0 0 1 2 0z"/>
                                </svg>
                            </div>

                            <input type="password"
                                className="form-control"
                                id="senha"
                                placeholder="Senha"
                                value={state.senha}
                                onChange={handleChange}
                            />
                        </div>
                    </div>
                    <div className="form-check">
                    </div>
                    <button
                        type="submit"
                        id="acessarlogin"
                        className="btn btn-primary"
                        onClick={handleSubmitClick}
                        disabled={loading}
                    >
                        {loading && <i className="fa fa-refresh fa-spin"></i>}
                        Login
                    </button>
                </form>
            </div>
        </section>
    )
}

export default Login;