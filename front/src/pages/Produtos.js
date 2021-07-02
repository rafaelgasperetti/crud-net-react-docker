import React, { useState, useEffect, createElement, useRef } from 'react';
import Header from '../components/Header';
import { obterProdutos, enviarProdutos } from '../services/produtos-api';
import { formatMoney, disconnect, getItemSession } from '../utils';
import { Loader } from '../components/Loader/Loader';

const Produtos = () => {
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

    const removeItem = (Id) => {
        setLoading(true);

        enviarProdutos(4, { Id: Id })
            .then(result => {
                fetchLocal();
            })
    }

    const novoItem = () => {
        window.location.href = "/produto/:id"
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

                <h4 className="titulo h4">Pesquisar</h4>
                <form className="form-cadastros" onSubmit={handleSubmit}>
                    <fieldset className="form-cadastros--wrapper">
                        <label className="input-label" htmlFor="propriedade-numero">Nome</label>
                        <input
                            type="text"
                            className="input"
                            placeholder="Busca do produto"
                            name="nome"
                            onChange={onInputChange}
                        />
                    </fieldset>
                    <button type="submit" className="btn btn--laranja btn--full">Filtrar</button>
                    <button type="button" className="btn btn--azl btn--full" onClick={novoItem}>Novo</button>
                </form>

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
                                        <th>Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {dados.map((dado, index) =>
                                        <tr key={`itemId${dado.Id}`}>
                                            <td>{dado.Nome}</td>
                                            <td>{dado.Descricao}</td>
                                            <td>{formatMoney(dado.Valor)}</td>
                                            <td>
                                                {createElement('a', { href: `/produto/${dado.Id}`, className: "c-roxo btn btn--link" }, <span className="fas fa-eye"></span>)}
                                            </td>
                                            <td className="text-center">
                                                <button type="button" onClick={() => { if (window.confirm('Deseja excluir esse item?')) removeItem(dado.Id) }} className="btn btn--icon tabela-items--delete"><span className="icon icon-trash"></span></button>
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

export default(Produtos);