---
id: dev-day-1
title: 개발 1일차 (Monolith, Clean Architecture, UserService)
description: TRB 프로젝트의 프론트 및 백엔드 구조 설계 기록
tags: [user-service, react, clean-architecture]
---

# 🛠 개발 1일차

## 📌 작업 내용

- React 프로젝트 생성 (Vite + TypeScript + Tailwind 기반)
- `MainPage`, `AuthPage` 생성 및 React Router 연동
- **모놀로식 + 클린 아키텍처 구조 설계**
- `User` 테이블 및 `UserRole` 테이블 생성 (MySQL)
- EF Core 대신 **ADO.NET 직접 쿼리 작성 방식으로 DB 연동**
- ConnectionFactory + Repository + Service 추상화 구현
- Docusaurus 문서 시스템 초기 세팅 완료

---

## 🧠 설계 철학

- 초기 단계에서는 **모놀로식 + 클린 아키텍처 구조**로 MVP 완성
- 각 레이어는 Domain / Application / Infrastructure / Presentation 으로 분리
- DB 연결은 `IDbConnectionFactory` 인터페이스를 통해 추상화
- EF Core 미사용, 모든 쿼리는 수동 작성 (MySqlConnection 기반)
- 프론트/백 통합 리포지토리 구조 (`src/client`, `src/server`)
- 문서는 GitHub Pages + Docusaurus 기반으로 지속 관리

---

## 🔁 향후 전환 계획

- MVP 완성 후 도메인별 서비스 분리 (UserService → MSA 분리)
- RabbitMQ, Redis, OpenSearch는 단계적으로 도입 예정
- 서비스 단위 Docker 구성, GitHub Actions 통한 자동 배포 연결

