---
id: dev-day-1
title: 개발 1일차 (React, MSA, UserService)
description: TRB 프로젝트의 프론트 및 백엔드 구조 설계 기록
tags: [user-service, react, msa]
---

# 🛠 개발 1일차

## 📌 작업 내용

- React 프로젝트 생성 (Vite 기반)
- MainPage / AuthPage 생성 및 라우팅 연결
- PowerShell 권한 문제 해결
- Visual Studio에서 UserService 백엔드 프로젝트 생성
- MSA 구조 기반 폴더/솔루션 구조 설계
- UserDbContext 기반 DB 연결 구조 설계
- GitHub Actions, Repo 분리 전략 구상
- DocuSaurus 도입 결정 및 초기 셋팅 완료

## 🧠 설계 철학

- 각 서비스는 독립적인 repo / 프로젝트 / Docker 컨테이너로 구성
- 공용 DBConnectionService는 사용하지 않으며, 서비스마다 DB 직접 관리
- 서비스 간 통신은 API 또는 메시지 큐로 구성 예정
- 모든 문서 기록은 DocuSaurus로 통합 관리
