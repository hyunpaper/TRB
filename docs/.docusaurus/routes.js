import React from 'react';
import ComponentCreator from '@docusaurus/ComponentCreator';

export default [
  {
    path: '/docs/__docusaurus/debug',
    component: ComponentCreator('/docs/__docusaurus/debug', 'e58'),
    exact: true
  },
  {
    path: '/docs/__docusaurus/debug/config',
    component: ComponentCreator('/docs/__docusaurus/debug/config', '2ce'),
    exact: true
  },
  {
    path: '/docs/__docusaurus/debug/content',
    component: ComponentCreator('/docs/__docusaurus/debug/content', '11b'),
    exact: true
  },
  {
    path: '/docs/__docusaurus/debug/globalData',
    component: ComponentCreator('/docs/__docusaurus/debug/globalData', 'f13'),
    exact: true
  },
  {
    path: '/docs/__docusaurus/debug/metadata',
    component: ComponentCreator('/docs/__docusaurus/debug/metadata', 'bff'),
    exact: true
  },
  {
    path: '/docs/__docusaurus/debug/registry',
    component: ComponentCreator('/docs/__docusaurus/debug/registry', '830'),
    exact: true
  },
  {
    path: '/docs/__docusaurus/debug/routes',
    component: ComponentCreator('/docs/__docusaurus/debug/routes', '13e'),
    exact: true
  },
  {
    path: '/docs/blog',
    component: ComponentCreator('/docs/blog', 'e9e'),
    exact: true
  },
  {
    path: '/docs/blog/archive',
    component: ComponentCreator('/docs/blog/archive', '5ff'),
    exact: true
  },
  {
    path: '/docs/blog/authors',
    component: ComponentCreator('/docs/blog/authors', '164'),
    exact: true
  },
  {
    path: '/docs/blog/authors/all-sebastien-lorber-articles',
    component: ComponentCreator('/docs/blog/authors/all-sebastien-lorber-articles', '5f1'),
    exact: true
  },
  {
    path: '/docs/blog/authors/yangshun',
    component: ComponentCreator('/docs/blog/authors/yangshun', 'f7a'),
    exact: true
  },
  {
    path: '/docs/blog/first-blog-post',
    component: ComponentCreator('/docs/blog/first-blog-post', '451'),
    exact: true
  },
  {
    path: '/docs/blog/long-blog-post',
    component: ComponentCreator('/docs/blog/long-blog-post', '135'),
    exact: true
  },
  {
    path: '/docs/blog/mdx-blog-post',
    component: ComponentCreator('/docs/blog/mdx-blog-post', '369'),
    exact: true
  },
  {
    path: '/docs/blog/tags',
    component: ComponentCreator('/docs/blog/tags', 'a37'),
    exact: true
  },
  {
    path: '/docs/blog/tags/docusaurus',
    component: ComponentCreator('/docs/blog/tags/docusaurus', '987'),
    exact: true
  },
  {
    path: '/docs/blog/tags/facebook',
    component: ComponentCreator('/docs/blog/tags/facebook', 'a94'),
    exact: true
  },
  {
    path: '/docs/blog/tags/hello',
    component: ComponentCreator('/docs/blog/tags/hello', '941'),
    exact: true
  },
  {
    path: '/docs/blog/tags/hola',
    component: ComponentCreator('/docs/blog/tags/hola', 'ae4'),
    exact: true
  },
  {
    path: '/docs/blog/welcome',
    component: ComponentCreator('/docs/blog/welcome', 'f3f'),
    exact: true
  },
  {
    path: '/docs/markdown-page',
    component: ComponentCreator('/docs/markdown-page', 'c78'),
    exact: true
  },
  {
    path: '/docs/',
    component: ComponentCreator('/docs/', '6fa'),
    exact: true
  },
  {
    path: '/docs/',
    component: ComponentCreator('/docs/', '4ad'),
    routes: [
      {
        path: '/docs/',
        component: ComponentCreator('/docs/', '31c'),
        routes: [
          {
            path: '/docs/tags',
            component: ComponentCreator('/docs/tags', 'fce'),
            exact: true
          },
          {
            path: '/docs/tags/architecture',
            component: ComponentCreator('/docs/tags/architecture', '926'),
            exact: true
          },
          {
            path: '/docs/tags/clean-architecture',
            component: ComponentCreator('/docs/tags/clean-architecture', 'da2'),
            exact: true
          },
          {
            path: '/docs/tags/monolith',
            component: ComponentCreator('/docs/tags/monolith', '3f9'),
            exact: true
          },
          {
            path: '/docs/tags/react',
            component: ComponentCreator('/docs/tags/react', '2eb'),
            exact: true
          },
          {
            path: '/docs/tags/structure',
            component: ComponentCreator('/docs/tags/structure', '10a'),
            exact: true
          },
          {
            path: '/docs/tags/user-service',
            component: ComponentCreator('/docs/tags/user-service', '8a5'),
            exact: true
          },
          {
            path: '/docs/',
            component: ComponentCreator('/docs/', '417'),
            routes: [
              {
                path: '/docs/architecture',
                component: ComponentCreator('/docs/architecture', '89f'),
                exact: true,
                sidebar: "defaultSidebar"
              },
              {
                path: '/docs/category/tutorial---extras',
                component: ComponentCreator('/docs/category/tutorial---extras', 'fc3'),
                exact: true,
                sidebar: "defaultSidebar"
              },
              {
                path: '/docs/dev-log/dev-day-1',
                component: ComponentCreator('/docs/dev-log/dev-day-1', 'deb'),
                exact: true,
                sidebar: "defaultSidebar"
              },
              {
                path: '/docs/intro',
                component: ComponentCreator('/docs/intro', '44d'),
                exact: true,
                sidebar: "defaultSidebar"
              },
              {
                path: '/docs/tutorial-extras/manage-docs-versions',
                component: ComponentCreator('/docs/tutorial-extras/manage-docs-versions', 'b22'),
                exact: true,
                sidebar: "defaultSidebar"
              },
              {
                path: '/docs/tutorial-extras/translate-your-site',
                component: ComponentCreator('/docs/tutorial-extras/translate-your-site', 'ae9'),
                exact: true,
                sidebar: "defaultSidebar"
              }
            ]
          }
        ]
      }
    ]
  },
  {
    path: '*',
    component: ComponentCreator('*'),
  },
];
