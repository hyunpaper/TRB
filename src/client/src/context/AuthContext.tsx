import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";

interface AuthContextType {
  token: string | null;
  email: string | null;
  role: string | null;
  nickname: string | null;
  profileImage: string | null;
  isLoggedIn: boolean;
  login: (
    token: string,
    email: string,
    role: string,
    nickname: string,
    profileImage: string | null
  ) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem("token"));
  const [email, setEmail] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const [nickname, setNickname] = useState<string | null>(null);
  const [profileImage, setProfileImage] = useState<string | null>(null);

  const login = (
    newToken: string,
    email: string,
    role: string,
    nickname: string,
    profileImage: string | null
  ) => {
    setToken(newToken);
    setEmail(email);
    setNickname(nickname);
    setProfileImage(profileImage);
    localStorage.setItem("token", newToken);

    // JWT 내부 roleId 해석
    try {
      const payload = JSON.parse(atob(newToken.split('.')[1]));
      const roleId = payload["roleId"];
      const roleMap: Record<string, string> = {
        "1": "사용자",
        "2": "관리자",
        "3": "게스트"
      };
      setRole(roleMap[roleId] || role);
    } catch {
      setRole(role);
    }
  };

  const logout = () => {
    setToken(null);
    setEmail(null);
    setRole(null);
    setNickname(null);
    setProfileImage(null);
    localStorage.removeItem("token");
  };

  const isLoggedIn = !!token;

  return (
    <AuthContext.Provider
      value={{
        token,
        email,
        role,
        nickname,
        profileImage,
        isLoggedIn,
        login,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within an AuthProvider");
  return context;
}
