import React from 'react';
import { useNavigate } from 'react-router-dom';

function MainPage() {
  const navigate = useNavigate();

  const handleLoginClick = () => {
    navigate('/auth'); // /auth로 이동
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '100px' }}>
      <h1>Welcome to the Service</h1>
      <button onClick={handleLoginClick} style={{ marginTop: '20px', padding: '10px 20px' }}>
        로그인
      </button>
    </div>
  );
}

export default MainPage;
