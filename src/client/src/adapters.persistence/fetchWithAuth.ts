export async function fetchWithAuth(input: RequestInfo, init?: RequestInit): Promise<Response> {
  let accessToken = localStorage.getItem("accessToken");
  const refreshToken = localStorage.getItem("refreshToken");
  const userId = JSON.parse(localStorage.getItem("user") || "{}").userId;

  const authInit: RequestInit = {
    ...init,
    headers: {
      ...(init?.headers || {}),
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "application/json"
    }
  };

  let response = await fetch(input, authInit);

  if (response.status === 401 && refreshToken && userId) {
    // AccessToken 만료 → refresh 시도
    const refreshResponse = await fetch("http://localhost:5186/api/auth/refresh", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userId, refreshToken })
    });

    if (refreshResponse.ok) {
      const data = await refreshResponse.json();
      accessToken = data.accessToken;
      localStorage.setItem("accessToken", accessToken ?? "");
      localStorage.setItem("refreshToken", data.refreshToken);

      // 원래 요청 재시도
      const retryInit: RequestInit = {
        ...init,
        headers: {
          ...(init?.headers || {}),
          Authorization: `Bearer ${accessToken}`
        }
      };

      return fetch(input, retryInit);
    } else {
      // refresh 실패 → 강제 logout
      localStorage.clear();
      window.location.href = "/login";
    }
  }

  return response;
}
