import React, { useState, useEffect, createElement, useRef } from 'react';
import Header from '../components/Header';
import { obterProdutos } from '../services/produtos-terceiros-api';
import { formatMoney, disconnect, getItemSession } from '../utils';
import { Loader } from '../components/Loader/Loader';

const ProdutosTerceiros = () => {
    if(!getItemSession('_token'))
    {
        window.location.href = "/login"
    }

    const [isLoading, setLoading] = useState(true);
    const [qtd, setQtd] = useState(0);
    const [dados, setDados] = useState([]);

    const [inputs, setInputs] = useState({
        nome: ''
    });

    const fetchLocal = (id, nome, descricao) => {
        setLoading(true);

        obterProdutos(id, nome, descricao)
            .then(result => {
                setDados(result.data.Data.Dados)
                setQtd(result.data.Data.QuantidadeRegistrosTotal)
                setLoading(false)
            })
            .catch(reject => {
                disconnect();
            })
    }

    useEffect(() => {
        fetchLocal();
    }, [])

    const onInputChange = event => {
        const { name, value } = event.target;

        setInputs({
            ...inputs,
            [name]: value
        });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        fetchLocal(null, inputs.nome, inputs.nome);
    }

    return (
        <>
            <Header />
            <section className="cadastros container">
                <div className="sect-header">
                    <div className="sect-titulo justify-content-between">
                        <h2 className="titulo h2">Produtos</h2>
                        <h6 className="titulo h6">Total de Produtos: <span className="arial-bold">{qtd}</span></h6>
                    </div>
                    <hr />
                </div>

                <h2 className="titulo h2 cadastros-titulo">Todos os Produtos</h2>
                <div className="tabela-overflow">
                    {!isLoading ? (
                        dados.length > 0 ? (
                            <table className="tabela tabela-listrada">
                                <thead>
                                    <tr>
                                        <th>Nome</th>
                                        <th>Descrição</th>
                                        <th>Valor</th>
                                        <th>Detalhe</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {dados.map((dado, index) =>
                                        <tr key={`itemId${dado.Id}`}>
                                            <td>{dado.Nome}</td>
                                            <td>{dado.Descricao}</td>
                                            <td>{formatMoney(dado.Valor)}</td>
                                            <td>
                                                {createElement('a', { href: `/produto-terceiro/${dado.Id}`, className: "c-roxo btn btn--link" }, <span className="fas fa-eye"></span>)}
                                            </td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        ) : (
                                <div className="alert alert-warning" role="alert">
                                    Nenhum produto encontrado
                                </div>
                            )

                    ) : (
                            <Loader short="false" />
                        )}

                </div>

                <div className="sect-footer align-items-start">
                    {createElement('a', { href: '/', className: "btn btn--cinza btn--block btn-full btn--bold" }, "Voltar")}
                </div>
            </section>
        </>
    )
}

export default(ProdutosTerceiros);