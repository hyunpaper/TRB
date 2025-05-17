export interface UserProfileUpdateRequestDto {
  birthDate?: string;       // yyyy-MM-dd 형식 (선택사항)
  gender?: string;          // "M" 또는 "F"
  address?: string;
  nickname?: string;
  profileImage?: string;    // 경로 or URL
}