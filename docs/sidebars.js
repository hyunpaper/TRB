module.exports = {
  tutorialSidebar: [
    {
      type: 'doc',
      id: 'architecture', // ✅ 이게 architecture.md의 frontmatter id
      label: '프로젝트 아키텍처',
    },
    {
      type: 'category',
      label: '개발 일지',
      items: [
        'dev-log/dev-day-1',
        'dev-log/dev-day-2',
        'dev-log/dev-day-3',
        'dev-log/dev-day-4',
      ]
    },
    {
      type: 'category',
      label: 'Api',
      items: [
        'api-login'
      ]
    },
  ]
};
