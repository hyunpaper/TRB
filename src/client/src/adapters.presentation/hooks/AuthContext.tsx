import { createContext, useContext, useEffect, useState } from "react";
import type { ReactNode } from "react";
import { fetchWithAuth } from "../../adapters.persistence/fetchWithAuth";

interface AuthContextType {
  token: string | null;
  email: string | null;
  role: string | null;
  nickname: string | null;
  profileImage: string | null;
  isLoggedIn: boolean;
  user: UserInfo | null;          
  fetchProfile: () => Promise<void>; 
  login: (
    accessToken: string,
    refreshToken: string,
    email: string,
    role: string,
    nickname: string,
    profileImage: string | null,
    roleId: number,
    userId: number,
  ) => void;
  logout: () => void;
}

interface UserInfo {
  email: string;
  name: string;
  nickname: string;
  profileImage: string;
  gender: string;
  address: string;
  role: string;
  brith: Date;
}


const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [email, setEmail] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const [nickname, setNickname] = useState<string | null>(null);
  const [profileImage, setProfileImage] = useState<string | null>(null);
  const [user, setUser] = useState<UserInfo | null>(null);
  // 🔄 localStorage에서 초기화
  useEffect(() => {
    const storedToken = localStorage.getItem("accessToken");
    const storedEmail = localStorage.getItem("email");
    const storedRole = localStorage.getItem("role");
    const storedNickname = localStorage.getItem("nickname");
    const storedProfileImage = localStorage.getItem("profileImage");

    if (storedToken) setToken(storedToken);
    if (storedEmail) setEmail(storedEmail);
    if (storedRole) setRole(storedRole);
    if (storedNickname) setNickname(storedNickname);
    if (storedProfileImage) setProfileImage(storedProfileImage);
  }, []);

  const login = (
    accessToken: string,
    refreshToken: string,
    email: string,
    role: string,
    nickname: string,
    profileImage: string | null,
    roleId: number,
    userId: number
  ) => {
    setToken(accessToken);
    setEmail(email);
    setRole(role);
    setNickname(nickname);
    setProfileImage(profileImage);

    // 🔐 저장
    localStorage.setItem("accessToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    localStorage.setItem("email", email);
    localStorage.setItem("role", role);
    localStorage.setItem("nickname", nickname);
    if (profileImage) localStorage.setItem("profileImage", profileImage);
    if (roleId !== undefined && roleId !== null) {
      localStorage.setItem("roleId", roleId.toString());
    }
    if (userId !== undefined && userId !== null) {
      localStorage.setItem("userId", userId.toString());
    }
  };

  const logout = () => {
    setToken(null);
    setEmail(null);
    setRole(null);
    setNickname(null);
    setProfileImage(null);

    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("email");
    localStorage.removeItem("role");
    localStorage.removeItem("nickname");
    localStorage.removeItem("profileImage");
    localStorage.removeItem("roleId");
    localStorage.removeItem("userId");
  };

const fetchProfile = async () => {
  const res = await fetchWithAuth("/api/user/profile");

  const rawText = await res.text();
  console.log("⚠️ 응답 텍스트:", rawText); // ✅ 실제 HTML인지 JSON인지 여기서 확인됨

  if (!res.ok) {
    console.error("❌ 서버 응답 오류:", res.status);
    throw new Error("프로필 불러오기 실패");
  }

  try {
    const profile = JSON.parse(rawText); // 안전하게 파싱 시도
    setUser({
      email: email ?? "",
      name: profile.name,
      nickname: profile.nickname,
      birthDate: profile.birth_date,
      gender: profile.gender,
      address: profile.address,
      profileImage: profile.profile_image,
      role: role ?? "",
    });
  } catch (e) {
    console.error("❌ JSON 파싱 실패:", e);
    throw e;
  }
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
        user,
        fetchProfile,
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
