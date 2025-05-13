---
title: Day 4 - RabbitMQ 기반 회원가입 구조 및 DLQ 처리 전략
sidebar_position: 4
---

# 🧪 Day 4 - 사용자 회원가입 비동기 처리 구조 설계

## ✅ 목표
RabbitMQ 기반의 회원가입 비동기 처리 구조 설계 및 구현.  
서비스 안정성과 장애 대응력을 확보하기 위한 **DLQ(Dead Letter Queue)** 및 **Retry Queue** 전략 수립.

---

## 🛠️ 작업 내역

### 1. RabbitMQ 구조 설계 및 도커화
- **RabbitMQ, MySQL**을 **각각의 Docker 컨테이너**로 구분하여 실행.
- RabbitMQ 설정:
  - 회원가입 처리 큐:  
    `user.signup.q1`, `user.signup.q2`, `user.signup.q3`  
    → **로드밸런싱 구조**로 분산 처리
  - DLQ (Dead Letter Queue):  
    `user.signup.retry` → 실패 메시지 재처리 전용 큐

### 2. DLQ 및 Retry 전략 수립
- 처리 실패 시:
  - 메시지를 `user.signup.retry`로 전송
  - `x-message-ttl = 10000` (10초 후)
  - `x-dead-letter-routing-key = user.signup.q{1~3}` 중 하나로 라운드로빈 방식 재전송
- 최대 재시도 후에도 실패 시:
  - **오류 로그 기록 (에러 메시지 포함)**
  - 메시지 삭제

### 3. 라운드로빈 큐 전송 방식 구현
- `user.signup.retry` → `user.signup.q1~3` 큐로 순차 전송 구현
- 추후 확장을 고려하여 **큐 이름 목록을 배열로 관리**하고 인덱스 순환 처리

### 4. 메시지 소비 구조
- `RabbitDequeuer` 클래스 내에서 메시지 소비 처리
- 예외 발생 시:
  - 실패 원인을 로그로 기록
  - 메시지는 retry 큐로 이동 → 일정 시간 후 재전송 → 최종 실패 시 삭제

---

## 📌 주요 고려 사항

- 서비스별로 큐를 세분화하지 않고, 하나의 서비스 내에서 큐 분산 처리로 관리 부담 최소화
- 에러 메시지를 분석하기 위해 영어 예외 메시지를 **한국어로 번역하여 저장**하는 방식 고려 중
- `.NET Framework 4.7.2` 환경에서도 적용 가능한 방식으로 구성

---

## 🧠 느낀 점 / 회고

- 로드밸런싱을 큐 단위로 처리하면서 **RabbitMQ의 구조적인 유연성**을 경험
- DLQ를 통해 장애 복구 가능성을 높이고, 운영자 입장에서 **문제 추적과 재시도 관리가 용이**
- 개발 초기 MVP 단계임에도 불구하고 **운영을 고려한 인프라 설계는 필수**

---

