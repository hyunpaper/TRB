---

id: api-login
title: 로그인 API
description: 사용자 로그인 요청 및 JWT 반환 구조
sidebar\_position: 1
tags: [api, user, login, jwt]
------------------------------

# 🔐 POST /api/user/login

사용자 로그인을 처리하고 JWT 토큰과 사용자 역할을 반환합니다.

---

## ✅ 요청 형식

* URL: `/api/user/login`
* Method: `POST`
* Content-Type: `application/json`

### 📥 Request Body

```json
{
  "email": "mw2815@naver.com",
  "password": "secure123"
}
```

---

## ✅ 응답 형식

### 📤 Response Body

```json
{
  "token": "<JWT 문자열>",
  "roleName": "관리자"
}
```

* `token`: JWT 형식의 access token (Bearer Authorization Header에 사용)
* `roleName`: 사용자 역할 이름 (예: "사용자", "관리자", "게스트")

---

## ⛔ 에러 응답 예시

### 📄 401 Unauthorized

```json
{
  "message": "이메일 또는 비밀번호가 잘못되었습니다."
}
```

또는:

```json
"Unauthorized"
```

---

## 📌 비고

* `token`은 추후 만료 시간(`exp`)이 포함되며, 보호된 API 접근 시 Authorization 헤더에 `Bearer`로 사용
* `roleName`은 클라이언트 UI 표시 및 권한 분기에 사용됨
