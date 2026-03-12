import React, { useState, useEffect, useRef } from 'react';
import './App.css';
import MainFooter from './Footer';
import MainHeader from './Header';
import { get } from 'http';

interface Questions {
  id: number;
  text: string;
  answer: string;
}
interface Answer {
  id: number;
  answer: string;
}
function sendAnswers(answers: Answer[]) {
    console.log('Sending answers:', answers);
} 
function App() {
  const [data, setData] = useState(null);
  const hasFetchedRef = useRef(false);

  useEffect(() => {
      if (hasFetchedRef.current) return; // guard: prevent 2nd StrictMode call
  hasFetchedRef.current = true;

    fetch('http://localhost:5210/Questions')
      .then((response) => response.json())
      .then((json) => setData(json))
      .then(() => console.log('json data:', data))
      .catch((error) => console.error(error));
  }, []);


  return (
    <div className="App flex flex-col min-h-screen bg-slate-50">
      <MainHeader />
      <main className="flex-1 container mx-auto px-4 py-12 w-full">
        <div className="bg-white rounded-lg shadow-md p-8">
          <h1>{JSON.stringify(data)}</h1>
        </div>
        <button onClick={() => sendAnswers([])}>Send Answers </button>
      </main>
      <MainFooter />
    </div>
  );
}

export default App;
