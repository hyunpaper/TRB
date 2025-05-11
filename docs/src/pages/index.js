import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';

export default function Home() {
  return (
    <Layout title="TRB 프로젝트 문서" description="실시간 암호화폐 분석 시스템 기술 문서">
      <main className="mx-auto max-w-3xl py-20 px-4 text-center">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">TRB Documentation 📘</h1>
        <p className="text-gray-700 mb-10 text-base">
          TRB는 실시간 암호화폐 시세 수집, 분석, 시각화를 위한 풀스택 프로젝트입니다.
        </p>

        <div className="flex flex-wrap justify-center gap-4">
          <Link
            to="/docs/architecture"
            className="px-6 py-3 border border-gray-300 rounded hover:bg-gray-100 transition"
          >
            🧱 아키텍처
          </Link>
          <Link
            to="/docs/dev-log/dev-day-2"
            className="px-6 py-3 border border-gray-300 rounded hover:bg-gray-100 transition"
          >
            📆 개발 일지
          </Link>
          <Link
            to="/docs/api-login"
            className="px-6 py-3 border border-gray-300 rounded hover:bg-gray-100 transition"
          >
            🔌 API 명세
          </Link>
        </div>
      </main>
    </Layout>
  );
}
