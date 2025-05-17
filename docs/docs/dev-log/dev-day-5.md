id: dev-day-5
title: 개발 5일차 (RabbitMQ 모듈화 및 메시지 처리 구조 정비)
description: 메시지 발행/소비 구조를 통합하고 라운드로빈 처리 방식 및 DLQ 흐름을 정비함
sidebar_position: 5
tags: [rabbitmq, messaging, consumer, dlq, retry]
🐇 개발 5일차 – RabbitMQ 모듈화 및 메시지 처리 구조 정비
✅ 주요 작업 요약
기존 RabbitMQ 메시지 발행/소비 구조를 통합 및 모듈화

메시지 발행 시 라운드로빈 방식으로 큐를 선택하도록 개선

소비자는 DLQ → Retry → 본 큐로 재전송되도록 자동 처리 흐름 구성

메시지 재처리에 실패한 경우 로그로 기록하고 폐기

💡 메시지 처리 구조 흐름
plaintext
복사
편집
[Client API 요청]
       ↓
[MessagePublisher] → [user.signup.q1/q2/q3 중 1곳 선택 발행]
       ↓
[RabbitDequeuer<T>] → 소비 실패 시 DLQ → Retry → 본 큐로 복귀 → 최종 실패 시 폐기
📦 구현 요소
1. RabbitEnqueuer<T>
위치: TRB.Server.Infrastructure.Messaging

메시지를 Serialize 후 큐에 발행

QueueNaming.GetQueueNameFor<T>()를 통해 라운드로빈 방식으로 큐를 선택

2. RabbitDequeuer<T>
위치: TRB.Server.Infrastructure.Messaging.Dequeuer

지정된 큐에 대해 메시지를 소비

실패 시 x-death 헤더를 통해 재시도 횟수 확인

TTL이 적용된 Retry 큐를 통해 본 큐로 자동 복귀

최대 3회 재시도 이후 메시지 폐기 및 로그 처리

3. QueueNaming
위치: TRB.Server.Infrastructure.Messaging

메시지 타입별 큐 이름 목록 관리

라운드로빈 큐 선택을 위한 내부 카운터 유지

⚙️ 메시지 발행 흐름
API에서 사용자 정보를 RabbitEnqueuer<UserSignupMessage>를 통해 발행

QueueNaming에 정의된 user.signup.q1~q3 중 하나를 자동 선택

RabbitMQ의 user.signup.{n} 큐에 메시지 전달됨

🔁 DLQ + Retry 처리 흐름
소비 실패 시 user.signup.dlq에 메시지 이동

10초 TTL이 지난 후 user.signup.retry 큐에서 다시 본 큐로 전달

Retry 횟수는 x-death 헤더로 체크

3회 초과 시 로그 기록 후 메시지 삭제 처리

📌 비고
개발 중에는 UserSignupConsumerService를 Presentation 내에 함께 포함시켜 테스트

이후 TRB.Worker 또는 TRB.Consumer 프로젝트로 분리 예정

메시지 분산 처리 및 재시도/폐기 흐름이 완성되었음