import { useState } from "react";
import { useAuth } from "../context/AuthContext";

interface AuthPageProps {
  mode?: "panel" | "page";
  onLoginSuccess?: () => void;
}

export default function AuthPage({ mode = "page", onLoginSuccess }: AuthPageProps) {
  const [isLoginMode, setIsLoginMode] = useState(true);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const { login } = useAuth();

  const toggleMode = () => setIsLoginMode(!isLoginMode);

  const isValidEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const handleSubmit = async () => {
    if (!isValidEmail(email)) {
      alert("유효한 이메일 형식이 아닙니다.");
      return;
    }

    const endpoint = isLoginMode ? "login" : "";
    const payload = { email, password };

    try {
      const response = await fetch(`http://localhost:5186/api/user/${endpoint}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const message = await response.text();
        alert(message);
        return;
      }

if (isLoginMode) {
  const result = await response.json();
  const token = result.token;
  const roleName = result.roleName; // ✅ 백엔드에서 온 roleName 받기
  login(token, email, roleName);    // ✅ AuthContext에 저장
  onLoginSuccess?.();
  alert("로그인 성공!");
} else {
        alert("회원가입 성공!");
        setIsLoginMode(true);
      }
    } catch (err) {
      alert("요청 중 오류가 발생하였습니다.");
      console.error(err);
    }
  };

  return (
    <div
      className={
        mode === "page"
          ? "min-h-screen flex items-center justify-center bg-gray-50"
          : ""
      }
    >
      <section
        className={
          mode === "page"
            ? "w-full max-w-md bg-white p-6 rounded-lg shadow-md"
            : "w-full"
        }
      >
        <h1 className="text-2xl font-semibold text-center mb-6">
          {isLoginMode ? "로그인" : "회원가입"}
        </h1>

        <form
          className="flex flex-col gap-4"
          onSubmit={(e) => e.preventDefault()}
        >
          <input
            type="email"
            placeholder="이메일"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
          />
          <input
            type="password"
            placeholder="비밀번호"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
          />

          <button
            type="submit"
            onClick={handleSubmit}
            className={`w-full ${
              isLoginMode
                ? "bg-blue-500 hover:bg-blue-600"
                : "bg-green-500 hover:bg-green-600"
            } text-white py-2 rounded transition`}
          >
            {isLoginMode ? "로그인" : "회원가입"}
          </button>
        </form>

        <p className="text-center mt-4 text-sm text-gray-600">
          {isLoginMode ? "계정이 없으신가요?" : "이미 계정이 있으신가요?"} {" "}
          <button
            onClick={toggleMode}
            className="text-blue-600 hover:underline font-medium"
          >
            {isLoginMode ? "회원가입" : "로그인"}
          </button>
        </p>
      </section>
    </div>
  );
}
