import { useState } from "react";

export default function AuthPanel() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleLogin = () => {
    alert(`로그인 시도: ${email}`);
    // TODO: 실제 토큰 연동
  };

  return (
    <div>
      <h2 className="text-lg font-bold mb-4">로그인</h2>
      <input
        type="email"
        placeholder="이메일"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        className="w-full border px-3 py-2 mb-3 rounded"
      />
      <input
        type="password"
        placeholder="비밀번호"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        className="w-full border px-3 py-2 mb-3 rounded"
      />
      <button
        onClick={handleLogin}
        className="w-full bg-blue-500 hover:bg-blue-600 text-white py-2 rounded"
      >
        로그인
      </button>
    </div>
  );
}
