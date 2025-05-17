import { useState, useEffect } from "react";

interface EditProfilePopupProps {
  email: string;
  name: string;
  nickname: string;
  birthDate: string;
  gender: string;
  address: string;
  profileImage: string;
  onClose: () => void;
  onSuccess: () => void;
}

export default function EditProfilePopup({
  email,
  name,
  nickname: initNickname,
  birthDate: initBirthDate,
  gender: initGender,
  address: initAddress,
  profileImage,
  onClose,
  onSuccess,
}: EditProfilePopupProps) {
  const [nickname, setNickname] = useState(initNickname);
  const [birthDate, setBirthDate] = useState(initBirthDate);
  const [gender, setGender] = useState(initGender);
  const [address, setAddress] = useState(initAddress);
  const [imageFile, setImageFile] = useState<File | null>(null);
 

  const handleSubmit = async () => {
    const formData = new FormData();
    formData.append("nickname", nickname);
    formData.append("birth_date", birthDate);
    formData.append("gender", gender);
    formData.append("address", address);
    if (imageFile) {
      formData.append("profile_image", imageFile);
    }

    try {
      const res = await fetch("/api/user/profile", {
        method: "PUT",
        body: formData,
        credentials: "include",
      });

      if (!res.ok) throw new Error("프로필 업데이트 실패");

      alert("프로필이 성공적으로 수정되었습니다.");
      onSuccess();
      onClose();
    } catch (err) {
      alert("프로필 수정 중 오류 발생");
      console.error(err);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center">
      <div className="bg-white p-8 rounded-xl w-full max-w-md shadow-lg space-y-4">
        <h2 className="text-xl font-semibold">프로필 수정</h2>

        <div>
          <label className="block text-sm font-medium mb-1">이메일</label>
          <input value={email} disabled className="w-full border rounded px-3 py-2 bg-gray-100" />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">이름</label>
          <input value={name} disabled className="w-full border rounded px-3 py-2 bg-gray-100" />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">닉네임</label>
          <input
            value={nickname}
            onChange={(e) => setNickname(e.target.value)}
            className="w-full border rounded px-3 py-2"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">생년월일</label>
          <input
            type="date"
            value={birthDate ?? ""}
            onChange={(e) => setBirthDate(e.target.value)}
            className="w-full border rounded px-3 py-2"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">성별</label>
          <select
            value={gender}
            onChange={(e) => setGender(e.target.value)}
            className="w-full border rounded px-3 py-2"
          >
            <option value="male">남성</option>
            <option value="female">여성</option>
            <option value="other">기타</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">주소</label>
          <input
            value={address}
            onChange={(e) => setAddress(e.target.value)}
            className="w-full border rounded px-3 py-2"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">프로필 이미지</label>
          <input type="file" onChange={(e) => setImageFile(e.target.files?.[0] || null)} />
        </div>

        <div className="flex justify-end gap-2 mt-4">
          <button onClick={onClose} className="px-4 py-2 bg-gray-300 rounded">
            취소
          </button>
          <button onClick={handleSubmit} className="px-4 py-2 bg-blue-600 text-white rounded">
            저장
          </button>
        </div>
      </div>
    </div>
  );
}
