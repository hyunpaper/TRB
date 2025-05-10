---
id: architecture
title: 프로젝트 구조 및 아키텍처
description: TRB 프로젝트의 폴더 구조 및 마이크로서비스 아키텍처 설계 개요
tags: [architecture, structure, msa]
---

# 🏗 TRB 프로젝트 아키텍처

## 📁 전체 폴더 구조

```plaintext
TRB/
├── client/                      # React 프론트엔드 (Vite)
├── service/                     # 각 도메인별 백엔드 서비스
│   ├── user-service/
│   │   └── UserService.sln + UserService/
│   ├── auth-service/
│   └── ...
├── docs/                        # Docusaurus 기반 문서 시스템
│   ├── dev-log/
│   └── api/
├── docker-compose.yml           # (선택) 전체 서비스 통합 실행
└── README.md
