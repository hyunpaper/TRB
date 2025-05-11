import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";

interface AuthContextType {
  token: string | null;
  email: string | null;
  role: string | null;
  isLoggedIn: boolean;
  login: (token: string, email: string, role: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem("token"));
  const [email, setEmail] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);

  const login = (newToken: string, email: string, role: string) => {
    setToken(newToken);
    setEmail(email);
    setRole(role); // ✅ 역할 저장
    localStorage.setItem("token", newToken);

    try {
      const payload = JSON.parse(atob(newToken.split('.')[1]));
      const roleId = payload["roleId"];
      const roleMap: Record<string, string> = {
        "1": "사용자",
        "2": "관리자",
        "3": "게스트"
      };
      setRole(roleMap[roleId] || "알 수 없음");
    } catch {
      setRole("알 수 없음");
    }
  };

  const logout = () => {
    setToken(null);
    setEmail(null);
    setRole(null);
    localStorage.removeItem("token");
  };

  const isLoggedIn = !!token;

  return (
    <AuthContext.Provider value={{ token, email, role, isLoggedIn, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within an AuthProvider");
  return context;
}
