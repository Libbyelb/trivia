import React, { useState, useEffect, use } from 'react';
import './App.css';
import MainFooter from './Footer';
import MainHeader from './Header';
import { get } from 'http';

interface Questions {
  id: number;
  text: string;
  answer: string;
}

function App() {
  const [data, setData] = useState(null);

  useEffect(() => {
    fetch('http://localhost:5210/Questions')
      .then((response) => response.json())
      .then((json) => setData(json))
      .then(() => console.log('Data fetched successfully'))
      .catch((error) => console.error(error));
  }, []);

async function get_question(): Promise<Questions[]> {
  const response = await fetch(`http://localhost:5210/Questions`);
  console.log(test);
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || response.statusText);
  }
  return response.json();
}
  return (
    <div className="App flex flex-col min-h-screen bg-slate-50">
      <MainHeader />
      <main className="flex-1 container mx-auto px-4 py-12 w-full">
        <div className="bg-white rounded-lg shadow-md p-8">
          <h1>{JSON.stringify(data)}</h1>
        </div>
        <button onClick={() => setData(null)}>Send Answers </button>
      </main>
      <MainFooter />
    </div>
  );
}

export default App;
