import React, { createElement, useEffect } from 'react';

const Unauthorized = () => {
    useEffect(() => {
        localStorage.clear();
    }, []);

    return (
        <>
            <section className="conteudo_interno container">
                <div className="text-center">
                    <h1 className="display-4">Oops!</h1>
                    <p>Ocorreu um erro inesperado.</p>
                    <p>Por favor entre em contato com o suporte.</p>

                    {process.env.REACT_APP_HOMOLOG === 'true' && createElement('a', { href: "/login", className: "mt-5 btn btn--cinza btn--block btn--full btn--bold" }, "Efetuar login")}
                </div>
            </section>
        </>
    )
}

export default Unauthorized;