// @ts-check
import { themes as prismThemes } from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'TRB Docs',
  url: 'https://hyunpaper.github.io',
  baseUrl: '/TRB/',
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  trailingSlash: false,
  favicon: 'img/favicon.ico',
  organizationName: 'hyunpaper',
  projectName: 'TRB',
  deploymentBranch: 'gh-pages',

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      },
    ],
  ],

  themeConfig: {
    navbar: {
      title: 'TRB Docs',
      logo: {
        alt: 'TRB Logo',
        src: 'img/logo.svg',
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
          href: 'https://github.com/hyunpaper/TRB',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
  },
};

export default config;
