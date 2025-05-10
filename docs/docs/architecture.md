---
id: architecture
title: 프로젝트 구조 및 아키텍처
description: TRB 프로젝트의 폴더 구조 및 클린 아키텍처 설계 개요
tags: [architecture, structure, monolith, clean-architecture]
---

# 🏗 TRB 프로젝트 아키텍처

> ✅ 현재 TRB 프로젝트는 **Monolithic + Clean Architecture** 기반으로 시작되며,  
> 향후 도메인 단위로 서비스를 분리(MSA 전환)할 계획입니다.

---

## 📁 전체 폴더 구조

```plaintext
TRB/
├── src/
│   ├── client/                  # React 프론트엔드 (Vite + TS + Tailwind)
│   └── server/                  # .NET 8 백엔드 (모놀로식 + 클린 아키텍처)
│       ├── TRB.Server.Domain/          # 엔티티, 인터페이스, 공통 규칙
│       ├── TRB.Server.Application/     # DTO, 서비스 인터페이스, 비즈니스 로직
│       ├── TRB.Server.Infrastructure/  # DB 연결, Repository 구현, 외부 연동
│       └── TRB.Server.Presentation/    # API Controller 및 Program.cs (EntryPoint)
├── docs/                        # Docusaurus 기반 문서 시스템
│   ├── dev-log/
│   └── api/
├── .github/workflows/           # GitHub Actions CI/CD 설정
└── README.md
