import React, { useState, useEffect, createElement } from 'react';

const NavBar = () => {
    return (
        <div className="header_nav d-none d-lg-block">
            <nav>
                <ul className="list-unstyled mb-0 nav justify-content-center nav-justified">
                    <li key={`menu-home`} className="nav_sub-menu_item">
                        {createElement('a', { href: `/`, className: "nav_sub-menu_link", key:`nav-home-a` }, `Home`)}
                    </li>
                    <li key={`menu-produtos`} className="nav_sub-menu_item">
                        {createElement('a', { href: `/produtos`, className: "nav_sub-menu_link", key:`nav-´produtos-a` }, `Produtos`)}
                    </li>
                    <li key={`menu-produtos-terceiros`} className="nav_sub-menu_item">
                        {createElement('a', { href: `/produtos-terceiros`, className: "nav_sub-menu_link", key:`nav-´produtos-a` }, `Produtos de Terceiros`)}
                    </li>
                </ul>
            </nav>
        </div>
    )
}

export default NavBar;