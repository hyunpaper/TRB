---

id: dev-day-3
title: 개발 3일차 (회원가입 메시지큐 처리 및 Consumer 구현)
description: TRB 백엔드에서 RabbitMQ를 통한 비동기 회원가입 처리 구현 기록
sidebar\_position: 3
tags: [rabbitmq, messaging, async, user-consumer]
--------------------------------------------------

# 📨 개발 3일차 – 메시지 큐 기반 회원가입 처리

## ✅ 주요 작업 요약

* 기존의 `POST /api/user` API는 직접 DB에 사용자 정보를 저장했음
* 이를 개선하여 RabbitMQ에 `UserSignupMessage` 메시지를 발행하도록 구조 변경
* 별도 Consumer(`UserSignupConsumer`)가 메시지를 수신하여 사용자 정보를 DB에 저장
* 메시지 수신은 실시간 구독 방식이 아닌, **5초 간격 스케줄링 디큐 방식**으로 처리
* 패스워드 해싱은 Controller가 아닌 Consumer 측에서 수행하도록 이동함

---

## 💡 기술 구조 변경 흐름

```plaintext
[Client] ---> [UserController] ---> [RabbitMQ Queue] ---> [UserSignupConsumer] ---> [DB]
```

---

## 📦 구현 요소

### 1. `UserSignupMessage`

* 위치: `TRB.Domain.Messages`

```csharp
public class UserSignupMessage
{
    public string Email { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}
```

### 2. `IUserSignupPublisher`

* 위치: `TRB.Application.Interfaces`
* RabbitMQ 발행용 인터페이스 정의

### 3. `RabbitMqUserSignupPublisher`

* 위치: `TRB.Infrastructure.Services`
* RabbitMQ 클라이언트로 메시지 발행 처리
* `RabbitMqOptions` 바인딩을 통해 구성값 주입 (HostName, Port, UserName 등)

### 4. `UserSignupConsumer`

* 위치: `TRB.Presentation.Consumers`
* 메시지를 5초 간격으로 수신(Polling 방식)
* 수신한 메시지로 비밀번호 해싱 후 사용자 저장
* 로그 출력은 `ILogger` 기반으로 처리

---

## ⚙️ 실행 흐름

1. 회원가입 요청 (`POST /api/user`)
2. 컨트롤러가 중복 이메일 체크 후 `UserSignupMessage` 생성
3. RabbitMQ에 메시지 발행
4. `UserSignupConsumer`가 5초 간격으로 큐를 확인
5. 메시지를 읽고 DB에 사용자 저장

---

## 🔧 설정 파일 예시 (appsettings.json)

```json
"RabbitMQ": {
  "HostName": "????",
  "Port": "????",
  "UserName": "????",
  "Password": "????"
}
```

---

## 📌 비고

* MVP 단계에서는 Consumer를 `TRB.Server.Presentation` 내부에 함께 포함하여 테스트 진행
* 운영 환경에선 `TRB.Consumer` 또는 `TRB.Worker` 프로젝트로 분리할 예정
* 메시지는 `trb.user.signup` 큐에 발행됨
* 로그 출력은 `ILogger<UserSignupConsumer>` 기반으로 수행됨
