import React, { useState, useEffect, createElement, useRef } from 'react';
// default
import Header from '../components/Header';
import { disconnect } from '../utils';
import { enviarProdutos, obterProdutos } from '../services/produtos-api';

const Produto = ({ match }) => {
    let id = match.params.id;

    const [data, setData] = useState([]);
    const [idData, setIdData] = useState([]);
    const [operacao, setOperacao] = useState([]);

    useEffect(() => {
        if(id != ":id") {
            setIdData(id);
            setOperacao(3);
            obterProdutos(idData, null, null, null)
                .then(result => {
                    setData(result.data.Data.Dados[0])
                })
                .catch(reject => {
                    disconnect();
                })
        } else {
            setIdData(null);
            setOperacao(2);
            setData({
                Nome: '',
                Descricao: '',
                Valor: 0
            });
        }
    }, [])

    const [inputs, setInputs] = useState({
        Nome: '',
        Descricao: '',
        Valor: 0
    });
    
    const onInputChange = event => {
        const { name, value } = event.target;

        setInputs({
            ...inputs,
            [name]: value
        });

        setData({
            ...data,
            [name]: value
        });
    };

    const onSubmit = event => {
        event.preventDefault();
        enviarProdutos(operacao, {
            Id: idData,
            Nome: data.Nome,
            Descricao: data.Descricao,
            Valor: data.Valor
        })
        .then(result => {
            if(result.data.Codigo == 200) {
                window.location.href = '/produtos';
            }
            else {
                alert(result.data.Status);
            }
        })
        .catch(reject => {
            disconnect();
        })
    };

    return (
        <>
            <Header />
            <section className="cadastros-detalhe container">
                <div className="sect-header">
                    <ul className="breadcrumb list-unstyled">
                        <li>
                            {/* <a href="index.html">Página Principal</a> */}
                            {createElement('a', { href: '/' }, "Página Principal")}
                        </li>
                        <li>
                            {createElement('a', { href: '/produtos' }, "Produtos")}
                        </li>
                        <li className="active">Detalhe do Produto</li>
                    </ul>
                    <div className="sect-titulo">
                        <h2 className="titulo h2">Produto <span className="c-laranja">{data.propriedade}</span></h2>
                    </div>
                </div>

                <form className="form" onSubmit={onSubmit}>
                    <div className="row cadastros-detalhe--wrapper">
                        <div className="col-md-5 col-lg-4 col-xl-3 cadastros-detalhe--item">
                            <span className="cadastros-detalhe--item-titulo">Nome:</span>
                            <input
                                type="text"
                                className="input"
                                placeholder="Nome"
                                name="Nome"
                                value={data.Nome || ''}
                                onChange={onInputChange}
                            />
                        </div>
                        <div className="col-6 col-md-4 col-lg-3 pecadastrosdidos-detalhe--item">
                            <span className="cadastros-detalhe--item-titulo">Descrição:</span>
                            <input
                                type="text"
                                className="input"
                                placeholder="Descrição"
                                name="Descricao"
                                value={data.Descricao || ''}
                                onChange={onInputChange}
                            />
                        </div>
                        <div className="col-6 col-md-3 col-lg-2 cadastros-detalhe--item">
                            <span className="cadastros-detalhe--item-titulo">Valor:</span>
                            <input
                                type="number"
                                className="input"
                                placeholder="Valor"
                                name="Valor"
                                step="any"
                                min="0"
                                value={data.Valor || 0}
                                onChange={onInputChange}
                            />
                        </div>
                    </div>

                    <div className="sect-footer align-items-start">
                        {createElement('a', { href: '/produtos', className: "btn btn--cinza btn--block btn-full btn--bold" }, "Voltar")}
                        <button type="submit" className="btn btn--laranja">Gravar</button>
                    </div>
                </form>
            </section>
        </>
    )
}

export default(Produto);