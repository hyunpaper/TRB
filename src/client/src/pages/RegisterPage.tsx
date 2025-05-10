import { useState } from "react";

export default function RegisterPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [roleId, setRoleId] = useState(1);

  const handleRegister = () => {
    alert(`회원가입 시도: ${email}, 역할: ${roleId}`);
    // TODO: API 연동
  };

  return (
    <main className="flex items-center justify-center min-h-screen bg-gray-50">
      <section className="w-full max-w-md bg-white p-6 rounded-lg shadow-md">
        <h1 className="text-2xl font-semibold text-center mb-6">회원가입</h1>

        <form className="space-y-4" onSubmit={(e) => e.preventDefault()}>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              이메일
            </label>
            <input
              type="email"
              className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="이메일 입력"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              비밀번호
            </label>
            <input
              type="password"
              className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="비밀번호 입력"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              역할 선택
            </label>
            <select
              className="w-full border border-gray-300 rounded px-3 py-2"
              value={roleId}
              onChange={(e) => setRoleId(Number(e.target.value))}
            >
              <option value={1}>사용자</option>
              <option value={2}>관리자</option>
              <option value={3}>게스트</option>
            </select>
          </div>

          <button
            type="submit"
            className="w-full bg-green-500 text-white py-2 rounded hover:bg-green-600 transition"
            onClick={handleRegister}
          >
            회원가입
          </button>
        </form>
      </section>
    </main>
  );
}
