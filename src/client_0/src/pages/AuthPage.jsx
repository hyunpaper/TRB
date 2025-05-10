import React, { useState } from 'react';

function AuthPage() {
  const [id, setId] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = () => {
    console.log('로그인 시도:', id, password);
    // TODO: 서버에 로그인 API 호출 예정
  };

  const handleFindId = () => {
    alert('아이디 찾기 기능은 아직 준비 중입니다.');
  };

  const handleSignup = () => {
    alert('회원가입 기능은 아직 준비 중입니다.');
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '100px' }}>
      <h1>로그인</h1>
      <div style={{ marginTop: '20px' }}>
        <input
          type="text"
          placeholder="아이디"
          value={id}
          onChange={(e) => setId(e.target.value)}
          style={{ padding: '10px', width: '250px', marginBottom: '10px' }}
        />
        <br />
        <input
          type="password"
          placeholder="비밀번호"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          style={{ padding: '10px', width: '250px', marginBottom: '20px' }}
        />
        <br />
        <button onClick={handleLogin} style={{ padding: '10px 20px', marginBottom: '10px' }}>
          로그인
        </button>
        <br />
        <button onClick={handleFindId} style={{ marginRight: '10px' }}>
          아이디 찾기
        </button>
        <button onClick={handleSignup}>
          회원가입
        </button>
      </div>
    </div>
  );
}

export default AuthPage;
