import React from 'react';
import {
  BrowserRouter,
  Switch,
  Route,
  Redirect
} from 'react-router-dom';

import Home from './pages/Home';
import Logout from './pages/Logout';
import NotFound from './pages/NotFound';
import Unauthorized from './pages/Unauthorized';
import Login from './pages/Login';
import Produtos from './pages/Produtos';
import Produto from './pages/Produto';
import ProdutosTerceiros from './pages/ProdutosTerceiros';
import ProdutoTerceiro from './pages/ProdutoTerceiro';
import { getItemSession } from './utils';

function App() {
  return (
    <BrowserRouter key={`main-browser-router`}>
      <Switch key="switch-main-route">
        <Route key={`login-route`} exact path="/login" component={Login} />
        <Route key={`not-found-route`} exact path="/not-found" component={NotFound} />
        <Route key={`unauthorized-route`} exact path="/unauthorized" component={Unauthorized} />
        <Route key={`main-route`} exact path="/" component={Home} />

        {getItemSession('_token') && [
          <>
            <Route key={`imoveis-route`} exact path="/produtos" component={Produtos} />
            <Route key={`imovel-route`} exact path="/produto/:id" component={Produto} />
            <Route key={`imoveis-route`} exact path="/produtos-terceiros" component={ProdutosTerceiros} />
            <Route key={`imovel-route`} exact path="/produto-terceiro/:id" component={ProdutoTerceiro} />
            <Route key={`logout-route`} exact path="/logout" component={Logout} />
          </>
        ]}

        <Route key="all_paths-route" path="*">
          <Redirect to="/unauthorized" />
        </Route>

      </Switch>
    </BrowserRouter>
  );
}

export default App;