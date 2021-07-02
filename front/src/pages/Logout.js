import React, { useEffect, useRef } from 'react';

const Logout = () => {

    useEffect(() => {
        sessionStorage.clear();
        localStorage.clear();
        window.location.href = '/login';
    }, [])

    return (
        <>
            <section className="conteudo_interno container">
                <div className="d-flex align-items-center">
                    <strong className="mr-2">Saindo... </strong>
                    <div className="spinner-border spinner-border-sm ml-auto" role="status" aria-hidden="true"></div>
                </div>
            </section>
        </>
    )

}

export default Logout;