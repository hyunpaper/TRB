---

id: dev-day-7
title: 개발 7일차 (실시간 전략 분석 결과 Redis 저장 및 배치 DB 연동)
description: 실시간 WebSocket 분석 결과를 Redis에 저장하고, 워커를 통한 DB 연동 구조 구현
sidebar\_position: 7
tags: \[websocket, redis, worker, strategy, batch]
--------------------------------------------------

# 📊 개발 7일차 – 실시간 전략 분석 Redis 저장 + 배치 DB 연동

---

## ✅ 주요 작업 요약

* 실시간 WebSocket으로 수신한 마켓 데이터를 분석하여 전략 결과 생성
* Redis에 전략 결과를 저장 (`latest:{market}` + `buffer:{market}`)
* GPT 로직은 현재 비활성화된 상태
* 별도 Worker에서 Redis 데이터를 30초 주기로 DB에 배치 저장

---

## 🔄 전략 분석 및 저장 흐름

```
[WebSocket 수신]
       ↓
[TickerData → StrategyAnalyzer]
       ↓
[StrategyAnalysisResultEntity 생성]
       ↓
Redis 저장:
  - strategy:latest:{market} (최신 1건 덮어쓰기)
  - strategy:buffer:{market} (배치용 append 리스트)
       ↓
[Worker] → 30초 주기로 buffer 읽어 DB에 저장
```

---

## 🧱 구현 구성 요소

### 1. `PriceWebSocketHandler`

* 위치: `TRB.Server.Presentation.WebSockets`
* WebSocket 수신 → `TickerData` 분석 → 전략 결과 생성
* `SaveLatestAsync(strategy)` 호출: 최신 전략 상태 Redis 저장
* `SaveToBufferAsync(strategy)` 호출: 무조건 버퍼에 저장 (GPT 없이)
* GPT 호출 로직은 현재 전부 주석 처리됨

### 2. `RedisService`

* 위치: `TRB.Server.Infrastructure.Services`
* `SaveLatestAsync`: `strategy:latest:{market}`에 덮어쓰기 저장
* `SaveToBufferAsync`: `strategy:buffer:{market}`에 append 저장 (GPT 없어도 무조건)

### 3. `RedisToDbBatchSaver`

* 위치: `TRB.Server.Worker.Services`
* 30초 주기로 모든 마켓의 `strategy:buffer:{market}` 키 확인
* Redis 리스트 전부 읽어 `StrategyAnalysisResultEntity`로 파싱
* `IStrategyDatabaseWriter.SaveAsync()` 호출해 DB 저장
* 저장 성공 시 Redis 키 삭제, 실패 시 유지 및 재시도

---

## 🧪 테스트 및 확인 항목

* ✅ Redis `LLEN strategy:buffer:{market}`로 데이터 쌓임 확인
* ✅ Worker 로그로 각 마켓별 저장 건수, 삭제 여부 출력 확인
* ✅ WebSocket 로그로 실시간 전략 분석 결과 출력됨
* ✅ Redis 값 파싱 실패 시 경고 로그 출력됨 (`null` 필터링 포함)

---

## 📌 비고 및 향후 작업

* 현재는 GPT 분석 없이 전략 결과만 저장
* 추후 GPT 추천 로직 재활성화 시 `SaveToBufferAsync` 조건부 로직 복원 예정
* Worker는 DLQ 처리, 재시도 제한, 최대 처리 건수 등 추가 개선 여지 있음
* 실시간 분석 → Redis 저장 → 워커를 통한 배치 저장 구조 정상 동작 확인됨
