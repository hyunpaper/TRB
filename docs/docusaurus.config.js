// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import {themes as prismThemes} from 'prism-react-renderer';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

module.exports = {
  title: 'TRB Docs',
  url: 'https://hyunpaper.github.io',         // GitHub Pages 도메인
  baseUrl: '/docs/',                          // 반드시 `/레포이름/`
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  trailingSlash: false, 
  favicon: 'img/favicon.ico',
  organizationName: 'hyunpaper',              // GitHub 유저명
  projectName: 'docs',                        // GitHub 레포 이름
  deploymentBranch: 'gh-pages',    


  presets: [
    [
      'classic',
      {
        docs: {
          routeBasePath: '/',                 // 옵션: 문서를 root(/)에 표시하려면 '/'
        },
      },
    ],
  ],

  
  themeConfig: {
    navbar: {
      title: 'TRB Docs', // 좌측 로고 텍스트
      logo: {
        alt: 'TRB Logo',
        src: 'img/logo.svg', // 기본 제공 이미지 경로 (원하면 img 폴더에 새 로고 넣을 수 있음)
      },
      items: [
        {
          type: 'doc',
          docId: 'intro', 
          position: 'left',
          label: '소개',
        },
        {
          type: 'doc',
          docId: 'architecture',
          position: 'left',
          label: '아키텍처',
        },
        {
          type: 'doc',
          docId: 'dev-log/dev-day-1',
          position: 'left',
          label: '개발 이력',
        },
        {
          href: 'https://github.com/your-org/trb',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
  },  
};

export default module.exports;
