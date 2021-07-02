import React from 'react';

export const Loader = (props) => {
    const short = props.short === true ? 'spinner-grow-sm' : '';
    return (
        <>
            <div className="d-flex justify-content-center">
                {/* <small className="text-muted">Carregando... </small> */}
                <div className={`spinner-grow ${short} text-secondary my-1`} role="status">
                    <span className="sr-only">Carregando...</span>
                </div>
            </div>
        </>
    )
}