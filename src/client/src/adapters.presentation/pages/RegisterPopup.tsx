// 📄 src/pages/RegisterPopup.tsx

import { useRef, useState } from "react";

export default function RegisterPopup({ onClose }: { onClose: () => void }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [name, setName] = useState("");
  const [phone, setPhone] = useState("");
  const [birthDate, setBirthDate] = useState("");
  const [gender, setGender] = useState("");
  const [address, setAddress] = useState("");
  const [nickname, setNickname] = useState("");
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const handleRegister = async () => {
    const formData = new FormData();
    formData.append("email", email);
    formData.append("password", password);
    formData.append("roleId", "1");
    formData.append("name", name);
    formData.append("phone", phone);
    formData.append("birthDate", birthDate);
    formData.append("gender", gender);
    formData.append("address", address);
    formData.append("nickname", nickname);

    const file = fileInputRef.current?.files?.[0];
    if (file) formData.append("profileImage", file);

    try {
      const response = await fetch("http://localhost:5186/api/user", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        const message = await response.text();
        alert(`회원가입 실패: ${message}`);
        return;
      }

      alert("회원가입 성공!");
      onClose();
    } catch (err) {
      alert("회원가입 중 오류 발생");
      console.error(err);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewUrl(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  return (
    <div className="bg-white w-[400px] p-6 rounded-lg shadow-xl relative">
      <button
        className="absolute top-2 right-2 text-gray-400 hover:text-black"
        onClick={onClose}
      >
        ✕
      </button>
      <h2 className="text-2xl font-bold mb-4">회원가입</h2>
      <div className="space-y-3">
        <input placeholder="이메일" value={email} onChange={(e) => setEmail(e.target.value)} className="w-full border px-3 py-2 rounded" />
        <input placeholder="비밀번호" type="password" value={password} onChange={(e) => setPassword(e.target.value)} className="w-full border px-3 py-2 rounded" />
        <input placeholder="이름" value={name} onChange={(e) => setName(e.target.value)} className="w-full border px-3 py-2 rounded" />
        <input placeholder="전화번호" value={phone} onChange={(e) => setPhone(e.target.value)} className="w-full border px-3 py-2 rounded" />
        <input type="date" value={birthDate} onChange={(e) => setBirthDate(e.target.value)} className="w-full border px-3 py-2 rounded" />
        <select value={gender} onChange={(e) => setGender(e.target.value)} className="w-full border px-3 py-2 rounded">
          <option value="">성별 선택</option>
          <option value="M">남성</option>
          <option value="F">여성</option>
          <option value="O">기타</option>
        </select>
        <input placeholder="주소" value={address} onChange={(e) => setAddress(e.target.value)} className="w-full border px-3 py-2 rounded" />
        <input placeholder="닉네임" value={nickname} onChange={(e) => setNickname(e.target.value)} className="w-full border px-3 py-2 rounded" />

        <div>
          <label className="block text-sm mb-1 text-gray-700">프로필 이미지 첨부</label>
          <input ref={fileInputRef} type="file" accept="image/*" onChange={handleFileChange} className="w-full border px-2 py-1 rounded" />
          {previewUrl && (
            <img src={previewUrl} alt="미리보기" className="mt-2 rounded w-24 h-24 object-cover border" />
          )}
        </div>

        <button
          onClick={handleRegister}
          className="w-full bg-gradient-to-r from-green-400 to-green-600 hover:from-green-500 hover:to-green-700 text-white font-semibold py-2 rounded shadow-md transition"
        >
          ✅ 회원가입 완료
        </button>
      </div>
    </div>
  );
}