id: dev-day-6
title: 개발 6일차 (JWT 기반 로그인 및 Redis를 통한 RefreshToken 저장)
description: JWT 로그인 발급 구조와 Redis 기반 RefreshToken 저장 전략 구현
sidebar_position: 6
tags: [jwt, redis, login, auth, refresh-token]
🔐 개발 6일차 – JWT 로그인 + Redis RefreshToken 저장
✅ 주요 작업 요약
로그인 성공 시 JWT AccessToken + RefreshToken을 생성하여 응답

AccessToken은 클라이언트에 저장, RefreshToken은 Redis에 저장

클라이언트에서는 AuthContext를 통해 로그인 상태를 전역 관리

새로고침 시에도 로그인 상태가 유지되도록 localStorage 활용

DashboardPage에서 로그인 여부에 따라 UI 동적으로 변경

💡 인증 흐름
plaintext
복사
편집
[Client Login 요청]
       ↓
[UserController] → JWT 생성 (access + refresh)
       ↓
AccessToken → 클라이언트 (localStorage)
RefreshToken → Redis 저장 (Key: refresh:{userId})
📦 구현 요소
1. JwtTokenService
위치: TRB.Server.Application.Services

JWT 생성 시 사용자 ID, Role, Email 등을 Claim에 포함

AccessToken(5분), RefreshToken(64byte 문자열) 발급

2. RedisTokenStore / RedisService
위치: TRB.Server.Infrastructure.Services

Redis에 refresh:{userId} 형식으로 저장

TTL은 7일 기준

3. AuthProvider.tsx
React 전역 Context로 로그인 상태 유지

login() 호출 시 사용자 정보 + 토큰 localStorage에 저장

새로고침 시 localStorage에서 토큰을 불러와 자동 로그인 처리

logout() 시 모든 상태 및 저장소 초기화

📦 예시 저장 항목 (로컬)
Key	Value
accessToken	JWT 문자열
refreshToken	랜덤 문자열
email	사용자 이메일
role	사용자 권한 (예: 사용자)
nickname	닉네임
profileImage	이미지 경로
roleId	숫자 권한 ID
userId	사용자 고유 ID

🧪 테스트 확인 항목
✅ 로그인 성공 시 access/refresh 토큰 발급됨

✅ Redis CLI에서 refresh:{userId} 키로 데이터 존재 확인

✅ 새로고침 후에도 로그인 상태 유지됨

✅ DashboardPage에서 사용자 정보 정상 출력됨

📌 비고
현재는 RefreshToken 검증 및 갱신 API는 미구현 상태

추후 /api/token/refresh 엔드포인트로 재발급 전략 구성 예정

Redis는 도커 컨테이너로 분리되어 독립적으로 운영 중