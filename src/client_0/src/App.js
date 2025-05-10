import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MainPage from './pages/MainPage';   // MainPage 컴포넌트 가져오기
import AuthPage from './pages/AuthPage';   // AuthPage 컴포넌트 가져오기

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<MainPage />} />   {/* 루트('/')로 오면 MainPage */}
        <Route path="/auth" element={<AuthPage />} /> {/* /auth로 오면 AuthPage */}
      </Routes>
    </Router>
  );
}

export default App;
