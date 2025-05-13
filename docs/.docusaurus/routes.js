import React from 'react';
import ComponentCreator from '@docusaurus/ComponentCreator';

export default [
  {
    path: '/TRB/__docusaurus/debug',
    component: ComponentCreator('/TRB/__docusaurus/debug', 'e68'),
    exact: true
  },
  {
    path: '/TRB/__docusaurus/debug/config',
    component: ComponentCreator('/TRB/__docusaurus/debug/config', '9f6'),
    exact: true
  },
  {
    path: '/TRB/__docusaurus/debug/content',
    component: ComponentCreator('/TRB/__docusaurus/debug/content', 'f1f'),
    exact: true
  },
  {
    path: '/TRB/__docusaurus/debug/globalData',
    component: ComponentCreator('/TRB/__docusaurus/debug/globalData', 'eb4'),
    exact: true
  },
  {
    path: '/TRB/__docusaurus/debug/metadata',
    component: ComponentCreator('/TRB/__docusaurus/debug/metadata', '8e6'),
    exact: true
  },
  {
    path: '/TRB/__docusaurus/debug/registry',
    component: ComponentCreator('/TRB/__docusaurus/debug/registry', 'bc9'),
    exact: true
  },
  {
    path: '/TRB/__docusaurus/debug/routes',
    component: ComponentCreator('/TRB/__docusaurus/debug/routes', 'c5f'),
    exact: true
  },
  {
    path: '/TRB/blog',
    component: ComponentCreator('/TRB/blog', '19a'),
    exact: true
  },
  {
    path: '/TRB/blog/archive',
    component: ComponentCreator('/TRB/blog/archive', '3b2'),
    exact: true
  },
  {
    path: '/TRB/blog/authors',
    component: ComponentCreator('/TRB/blog/authors', 'd03'),
    exact: true
  },
  {
    path: '/TRB/blog/authors/all-sebastien-lorber-articles',
    component: ComponentCreator('/TRB/blog/authors/all-sebastien-lorber-articles', '244'),
    exact: true
  },
  {
    path: '/TRB/blog/authors/yangshun',
    component: ComponentCreator('/TRB/blog/authors/yangshun', 'a77'),
    exact: true
  },
  {
    path: '/TRB/blog/first-blog-post',
    component: ComponentCreator('/TRB/blog/first-blog-post', '81e'),
    exact: true
  },
  {
    path: '/TRB/blog/long-blog-post',
    component: ComponentCreator('/TRB/blog/long-blog-post', '659'),
    exact: true
  },
  {
    path: '/TRB/blog/mdx-blog-post',
    component: ComponentCreator('/TRB/blog/mdx-blog-post', '160'),
    exact: true
  },
  {
    path: '/TRB/blog/tags',
    component: ComponentCreator('/TRB/blog/tags', '43c'),
    exact: true
  },
  {
    path: '/TRB/blog/tags/docusaurus',
    component: ComponentCreator('/TRB/blog/tags/docusaurus', '632'),
    exact: true
  },
  {
    path: '/TRB/blog/tags/facebook',
    component: ComponentCreator('/TRB/blog/tags/facebook', 'bea'),
    exact: true
  },
  {
    path: '/TRB/blog/tags/hello',
    component: ComponentCreator('/TRB/blog/tags/hello', '0e9'),
    exact: true
  },
  {
    path: '/TRB/blog/tags/hola',
    component: ComponentCreator('/TRB/blog/tags/hola', '6a9'),
    exact: true
  },
  {
    path: '/TRB/blog/welcome',
    component: ComponentCreator('/TRB/blog/welcome', 'f86'),
    exact: true
  },
  {
    path: '/TRB/markdown-page',
    component: ComponentCreator('/TRB/markdown-page', '1c1'),
    exact: true
  },
  {
    path: '/TRB/docs',
    component: ComponentCreator('/TRB/docs', '3cf'),
    routes: [
      {
        path: '/TRB/docs',
        component: ComponentCreator('/TRB/docs', '71a'),
        routes: [
          {
            path: '/TRB/docs/tags',
            component: ComponentCreator('/TRB/docs/tags', '93a'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/api',
            component: ComponentCreator('/TRB/docs/tags/api', 'fc8'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/architecture',
            component: ComponentCreator('/TRB/docs/tags/architecture', '180'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/async',
            component: ComponentCreator('/TRB/docs/tags/async', 'f3d'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/auth',
            component: ComponentCreator('/TRB/docs/tags/auth', '8de'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/clean-architecture',
            component: ComponentCreator('/TRB/docs/tags/clean-architecture', '57c'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/jwt',
            component: ComponentCreator('/TRB/docs/tags/jwt', '59f'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/login',
            component: ComponentCreator('/TRB/docs/tags/login', 'dd1'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/messaging',
            component: ComponentCreator('/TRB/docs/tags/messaging', '2d2'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/monolith',
            component: ComponentCreator('/TRB/docs/tags/monolith', '692'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/rabbitmq',
            component: ComponentCreator('/TRB/docs/tags/rabbitmq', 'b1b'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/react',
            component: ComponentCreator('/TRB/docs/tags/react', 'b32'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/react-context',
            component: ComponentCreator('/TRB/docs/tags/react-context', 'cbf'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/structure',
            component: ComponentCreator('/TRB/docs/tags/structure', '817'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/user',
            component: ComponentCreator('/TRB/docs/tags/user', 'd7c'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/user-consumer',
            component: ComponentCreator('/TRB/docs/tags/user-consumer', '0b2'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/user-role',
            component: ComponentCreator('/TRB/docs/tags/user-role', '4a0'),
            exact: true
          },
          {
            path: '/TRB/docs/tags/user-service',
            component: ComponentCreator('/TRB/docs/tags/user-service', 'e56'),
            exact: true
          },
          {
            path: '/TRB/docs',
            component: ComponentCreator('/TRB/docs', '1a5'),
            routes: [
              {
                path: '/TRB/docs/api-login',
                component: ComponentCreator('/TRB/docs/api-login', 'c72'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/TRB/docs/architecture',
                component: ComponentCreator('/TRB/docs/architecture', '8be'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/TRB/docs/dev-log/dev-day-1',
                component: ComponentCreator('/TRB/docs/dev-log/dev-day-1', '18c'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/TRB/docs/dev-log/dev-day-2',
                component: ComponentCreator('/TRB/docs/dev-log/dev-day-2', 'b5e'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/TRB/docs/dev-log/dev-day-3',
                component: ComponentCreator('/TRB/docs/dev-log/dev-day-3', 'e7d'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/TRB/docs/dev-log/dev-day-4',
                component: ComponentCreator('/TRB/docs/dev-log/dev-day-4', '8c3'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/TRB/docs/intro',
                component: ComponentCreator('/TRB/docs/intro', 'c35'),
                exact: true
              }
            ]
          }
        ]
      }
    ]
  },
  {
    path: '/TRB/',
    component: ComponentCreator('/TRB/', '4ae'),
    exact: true
  },
  {
    path: '*',
    component: ComponentCreator('*'),
  },
];
