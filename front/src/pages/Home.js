import { result } from 'lodash';
import React, { useState, useEffect } from 'react';
import Header from '../components/Header';
import { obterContagemProdutos } from '../services/produtos-api';
import { disconnect, getItemSession } from '../utils';

const Home = ({ refreshCartFn }) => {
    if(!getItemSession('_token'))
    {
        window.location.href = "/login"
    }

    const [qtdProdutos, setQtdProdutos] = useState(0);

    const fetchLocal = () => {
        obterContagemProdutos()
            .then(result => {
                setQtdProdutos(result.data.Data.QuantidadeRegistrosTotal);
            })
            .catch(reject => {
                disconnect();
            });
    }

    useEffect(() => {
        fetchLocal();
    }, [])

    return (
        <>
            <Header />
            <section className="cadastros container">
                <div className="sect-header">
                    <div className="sect-titulo justify-content-between">
                        <h6 className="titulo h6">Total de Produtos: <span className="arial-bold">{qtdProdutos}</span></h6>
                    </div>
                </div>
            </section>
        </>
    )
}

export default(Home);