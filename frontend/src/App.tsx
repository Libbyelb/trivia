import { useState, useEffect, useRef } from 'react';
import './App.css';
import MainFooter from './Footer';
import MainHeader from './Header';
import { Dropdown, IDropdownStyles, IDropdownOption, ColorClassNames } from '@fluentui/react';

interface Questions {
  Type: string;
  Difficulty: string;
  Category: string;
  Question: string;
  Answers: string[];
  CorrectAnswers: string[];
}


function App() {
  const [data, setData] = useState<Questions[] | null>(null);
  const hasFetchedRef = useRef(false);
  const [selectedAnswers, setSelectedAnswers] = useState<Record<number, string>>({});
  const [score, setScore] = useState<boolean[] | null>(null);
  useEffect(() => {
    if (hasFetchedRef.current) return; // guard: prevent 2nd StrictMode call
    hasFetchedRef.current = true;

    fetch('http://localhost:5210/GetQuestions')
      .then((response) => response.json())
      .then((json: Questions[]) => {
        setData(json);
        console.log('json data:', json);
      })
      .catch((error) => console.error(error));
  }, []);

  const dropdownStyles: Partial<IDropdownStyles> = {
    dropdown: { width: 300 },
  };


  const handleAnswerChange = (questionIndex: number, option?: IDropdownOption) => {
    if (!option) return;
    setSelectedAnswers((prev) => ({
      ...prev,
      [questionIndex]: String(option.text)
    }));
  };

  const sendAnswers = async () => {
    const questions = (data ?? []).map((q) => q.Question);
    const selectedAnswersArray = questions.map((_, i) => selectedAnswers[i] ?? '');
    const correctAnswers = (data ?? []).map((q) => q.CorrectAnswers[0]);

    const response = await fetch('http://localhost:5210/checkanswers', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ questions, answers: selectedAnswersArray, correctAnswers }),
    });
    const result = (await response.json()) as boolean[];
    setScore(result)
    console.log('Response:', result);
  };

  return (
    <div className="App flex flex-col min-h-screen bg-slate-50">
      <MainHeader />
      <main className="flex-1 container mx-auto px-4 py-12 w-full">
        <div className="bg-white rounded-lg shadow-md p-8">
          <div>
            {data?.map((question, index) => (
              <div key={index}>
                {question.Type === "multiple" ? (
                  <div>
                    <Dropdown  style ={{ marginLeft: '150px' }}
                      placeholder="Select your answer"
                      label={question.Question}
                      onChange={(_, option) => handleAnswerChange(index, option)}
                      options={question.Answers.map((answer, idx) => ({ key: idx, text: answer }))}
                      styles={dropdownStyles}
                    />
                  </div>
                ) : (
                  <Dropdown  style ={{ marginLeft: '150px' }}
                    placeholder="Select your answer"
                    label={question.Question}
                    onChange={(_, option) => handleAnswerChange(index, option)}
                    options={[
                      { key: 'true', text: 'True' },
                      { key: 'false', text: 'False' }
                    ]}
                    styles={dropdownStyles}
                  />
                  
                )}
                             {
                    score === null ? null :
                    score[index] === true ? <p className="text-green-500" style={{ color: 'green' }}>Correct answer!</p> : 
                    score[index] === false ? <p className="text-red-500" style={{ color: 'red' }}>Incorrect answer.</p> : 
                    null}
              </div>
            ))}
            
          </div>
        </div>
        <button onClick={sendAnswers}>Send Answers</button>
      </main>
      <MainFooter />
    </div>
  );
}

export default App;
