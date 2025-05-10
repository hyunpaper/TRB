# 📘 TRB 프로젝트 문서

PROJECT TRB의 기술 문서  
아키텍처, 사용 스택, 브랜치 전략, 개발 이력 기록용  
**GitHub Pages + Docusaurus** 기반으로 배포함

🔗 [📂 문서 사이트 바로가기](https://hyunpaper.github.io/TRB/)

---

## 📦 사용 기술 스택

| 항목 | 내용 |
|:--|:--|
| Frontend | React (Vite + TypeScript + Tailwind CSS) |
| Backend | .NET 8 Web API (C#) |
| DB | MySQL (ADO.NET 직접 쿼리 작성, EF Core 미사용) |
| Messaging | RabbitMQ (예정) |
| Cache | Redis (예정) |
| Search Engine | OpenSearch (예정) |
| Infra | Docker, GitHub Actions |
| Architecture | Monolithic → Clean Architecture 기반 설계 |
| CI/CD | GitHub Actions + GitHub Flow |

---

## 📁 주요 문서 링크

| 문서 | 설명 |
|:--|:--|
| [🧱 아키텍처 개요](https://hyunpaper.github.io/docs/architecture) | Clean Architecture 기반 구성 설명 |
| [🗂 개발 이력 - Day 1](https://hyunpaper.github.io/docs/dev-log/dev-day-1) | 초기 프로젝트 구조 세팅, API 연동 기록 |
| [🌳 브랜치 전략](https://hyunpaper.github.io/docs/strategy/branch) ✅ 작성 완료 | GitHub Flow 전략 설명 |
| [🧪 API 명세](https://hyunpaper.github.io/docs/api/user-service) ✅ 작성 완료 | UserService - 회원가입/조회 API 구조 |

---

## 🚀 문서 로컬 실행 방법

```bash
npm install
npm start
