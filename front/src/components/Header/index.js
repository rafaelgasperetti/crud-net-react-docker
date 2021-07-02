import React from 'react';
import NavBar from '../NavBar';
import '../../scss/common.scss';

export default function Header() {
  return (
    <>
      <header className="header desktop">
        <NavBar />
      </header>
    </>
  );
}
