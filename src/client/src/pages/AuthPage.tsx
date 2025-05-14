// 📄 src/pages/AuthPage.tsx

import { useState } from "react";
import { useAuth } from "../context/AuthContext";

interface AuthPageProps {
  mode?: "panel" | "page";
  onLoginSuccess?: () => void;
  onShowRegister?: () => void;
}

export default function AuthPage({ mode = "page", onLoginSuccess, onShowRegister }: AuthPageProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const { login } = useAuth();

  const isValidEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const handleSubmit = async () => {
    if (!isValidEmail(email)) {
      alert("유효한 이메일 형식이 아닙니다.");
      return;
    }

    const payload = { email, password };

    try {
      const response = await fetch("http://localhost:5186/api/user/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const message = await response.text();
        alert(message);
        return;
      }

      const result = await response.json();

      const token = result.token;
      const email = result.user.email;
      const roleName = result.user.role_name;
      const nickname = result.user.nickname;
      const profileImage = result.user.profileImage;

login(token, email, roleName, nickname, profileImage);
      
      login(token, email, roleName,nickname,profileImage);
      onLoginSuccess?.();
      alert("로그인 성공!");
      console.log("로그인 응답:", result);
    } catch (err) {
      alert("요청 중 오류가 발생하였습니다.");
      console.error(err);
    }
  };

  return (
    <div className={mode === "page" ? "min-h-screen flex items-center justify-center bg-gray-50" : ""}>
      <section className={mode === "page" ? "w-full max-w-md bg-white p-6 rounded-lg shadow-md" : "w-full"}>
        <h1 className="text-2xl font-semibold text-center mb-6">로그인</h1>

        <form className="flex flex-col gap-4" onSubmit={(e) => e.preventDefault()}>
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
            className="w-full bg-blue-500 hover:bg-blue-600 text-white py-2 rounded transition"
          >
            로그인
          </button>
        </form>

        <p className="text-center mt-4 text-sm text-gray-600">
          계정이 없으신가요? {" "}
          <button
            onClick={() => {
              if (mode === "panel") {
                onShowRegister?.();
              } else {
                alert("회원가입은 별도 페이지 또는 팝업에서 처리됩니다.");
              }
            }}
            className="text-blue-600 hover:underline font-medium"
          >
            회원가입
          </button>
        </p>
      </section>
    </div>
  );
}
