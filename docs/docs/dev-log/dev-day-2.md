---

id: dev-day-2
title: 개발 2일차 (JWT 인증, 사용자 역할, 전역 상태)
description: TRB 프로젝트의 인증 시스템 구축 및 역할 기반 렌더링 처리 기록
tags: [auth, jwt, react-context, user-role]
--------------------------------------------

# 🔐 개발 2일차

## ✅ 작업 내용 요약

* JWT 기반 로그인 처리 구현
* 로그인 시 서버에서 `token`과 `roleName`을 함께 반환하도록 설정
* 클라이언트에서 `AuthContext`로 로그인 상태, 이메일, 역할 정보를 전역으로 관리
* 로그인 이후 UI 전환 (오른쪽 로그인 패널 → 환영 메시지 + 로그아웃 버튼)
* 상세 분석 기능은 로그인 사용자만 접근 가능하도록 제어
* JWT에서 직접 `roleId` 디코딩 대신 서버에서 `roleName` 응답 방식 선택
* 사용자 역할은 `사용자`, `관리자`, `게스트` 중 하나로 매핑

---

## 🧠 기술적 결정

* `roleId` → `roleName` 매핑은 클라이언트가 아니라 서버가 처리
* 클라이언트는 역할 이름을 그대로 받아 사용하여 유지보수성과 가독성 확보
* 역할 전역 관리 구조는 `AuthContext`에 `role`로 포함하여 React 앱 전체에서 접근 가능

---

## 📄 코드 흐름 요약

### 1. 서버 응답 예시

```json
{
  "token": "<JWT_STRING>",
  "roleName": "관리자"
}
```

### 2. 클라이언트 처리

```tsx
const result = await response.json();
const token = result.token;
const roleName = result.roleName;
login(token, email, roleName);
```

### 3. AuthContext 구조

```tsx
interface AuthContextType {
  token: string | null;
  email: string | null;
  role: string | null;
  ...
}
```

---

## 📌 UI 동작

* 로그인 전: 이메일/비밀번호 입력폼 + 회원가입 전환 버튼
* 로그인 후: `email · 역할명` 표시 + 로그아웃 버튼 표시

---

## 🔒 보호 기능

* 상세 분석 기능 버튼은 `isLoggedIn === false`인 경우 클릭 시 alert 표시
* 로그인된 사용자만 `ChartAnalysisModal` 열람 가능

---


## 🧩 사용자 인증 관련 DB 구조 (ERD)

### 📋 User 테이블

| 컬럼명    | 타입        | 설명               |
|-----------|-------------|--------------------|
| user_id   | INT (PK)    | 사용자 고유 ID     |
| email     | VARCHAR     | 사용자 이메일      |
| password  | VARCHAR     | 해시된 비밀번호    |
| role_id   | INT (FK)    | 역할 ID (UserRole) |
| created_at| DATETIME    | 생성 일시          |
| enabled   | BOOLEAN/CHAR| 활성 여부          |

### 📋 UserRole 테이블

| 컬럼명    | 타입     | 설명         |
|-----------|----------|--------------|
| role_id   | INT (PK) | 역할 ID      |
| role_name | VARCHAR  | 역할 이름 (ex. 사용자, 관리자) |

---

**🔗 관계:** `User.role_id → UserRole.role_id` (N:1)


## 📁 연관된 파일 목록

* `AuthContext.tsx`
* `AuthPage.tsx`
* `Dashboard.tsx`
* `CoinChart.tsx`
* `UserController.cs` (`role_name` 포함 응답 처리)
