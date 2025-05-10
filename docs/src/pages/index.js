import React from 'react';
import Layout from '@theme/Layout';

export default function Home() {
  return (
    <Layout
      title={`TRB 개발 문서`}
      description="TRB 프로젝트 문서 시스템">
      <main style={{ textAlign: 'center', padding: '5rem 0' }}>
        <h1>🚀 Welcome to TRB Docusaurus!</h1>
        <p>FE, BE, Architecture, Dev history 등을 이곳에서 관리합니다.</p>
        <a href="/docs/architecture">
          <button style={{ padding: '1rem 2rem', fontSize: '1.2rem', marginTop: '2rem' }}>
            아키텍처 문서 보기 →
          </button>
        </a>
      </main>
    </Layout>
  );
}
