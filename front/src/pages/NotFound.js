import React, { createElement } from 'react';
// import { Link } from 'react-router-dom';

const NotFound = () => {
    return (
        <>
            <section className="conteudo_interno container">
                <div>
                    <h1 class="display-4">Oops!<br />Página no encontrada</h1>
                    {/* <Link to="/" class="btn btn--cinza btn--block btn--full btn--bold">Ir para página inicial</Link> */}
                    {createElement('a', { href: "/", className: "mt-5 btn btn--cinza btn--block btn--full btn--bold" }, "Ir a la página de inicio")}
                </div>
            </section>
        </>
    )
}

export default NotFound;