import React from 'react';
import logo from './logo.svg';
import './App.css';
import MainFooter from './Footer';
import MainHeader from './Header';
function App() {
  return (
    <div className="App">
      <MainHeader />
      <main className="container mx-auto py-8">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </main>
      <MainFooter />
    </div>
  );
}

export default App;
