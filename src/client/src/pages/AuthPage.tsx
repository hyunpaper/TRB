import { useState } from "react";

export default function AuthPage() {
  const [isLoginMode, setIsLoginMode] = useState(true);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  

const toggleMode = () => setIsLoginMode(!isLoginMode);

const handleSubmit = async () => {
  // 이메일 정규식 검사 함수
  const isValidEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  if (isLoginMode) {
    try {
      const response = await fetch("http://localhost:5186/api/user/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email,
          password,
        }),
      });

      if (!response.ok) {
        if (response.status === 401) {
          alert("이메일 또는 비밀번호가 잘못되었습니다.");
        } else {
          throw new Error("로그인 실패");
        }
        return;
      }

      alert("로그인 성공!");
      // TODO: 토큰 저장, 라우팅 등 처리
    } catch (err) {
      alert("로그인 중 오류가 발생하였습니다.");
      console.error(err);
    }
  } else {
    //  회원가입 시 이메일 형식 검사 추가
    if (!isValidEmail(email)) {
      alert("유효한 이메일 형식이 아닙니다.");
      return;
    }

    try {
      const response = await fetch("http://localhost:5186/api/user", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email,
          password,
          roleId: 1,
        }),
      });

      if (!response.ok) {
        if (response.status === 409) {
          const message = await response.text();
          alert(message); // "이미 등록된 이메일입니다."
          return;
        }

        throw new Error("회원가입 실패");
      }

      alert("회원가입 성공!");
      setIsLoginMode(true);
    } catch (err) {
      alert("회원가입 중 오류 발생");
      console.error(err);
    }
  }
};



  return (
    <main className="flex items-center justify-center min-h-screen bg-gray-50">
      <section className="w-full max-w-md bg-white p-6 rounded-lg shadow-md">
        <h1 className="text-2xl font-semibold text-center mb-6">
          {isLoginMode ? "로그인" : "회원가입"}
        </h1>

        <form className="space-y-4" onSubmit={(e) => e.preventDefault()}>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">이메일</label>
            <input
              type="email"
              className="w-full border border-gray-300 rounded px-3 py-2 focus:ring-2 focus:ring-blue-500"
              placeholder="이메일 입력"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">비밀번호</label>
            <input
              type="password"
              className="w-full border border-gray-300 rounded px-3 py-2 focus:ring-2 focus:ring-blue-500"
              placeholder="비밀번호 입력"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button
            type="submit"
            className={`w-full ${
              isLoginMode ? "bg-blue-500 hover:bg-blue-600" : "bg-green-500 hover:bg-green-600"
            } text-white py-2 rounded transition`}
            onClick={handleSubmit}
          >
            {isLoginMode ? "로그인" : "회원가입"}
          </button>
        </form>

        <p className="text-center mt-4 text-sm text-gray-600">
          {isLoginMode ? "계정이 없으신가요?" : "이미 계정이 있으신가요?"}{" "}
          <button
            onClick={toggleMode}
            className="text-blue-600 hover:underline font-medium"
          >
            {isLoginMode ? "회원가입" : "로그인"}
          </button>
        </p>
      </section>
    </main>
  );
}
