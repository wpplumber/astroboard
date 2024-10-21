// @ts-check
import { defineConfig, squooshImageService } from 'astro/config';
import starlight from '@astrojs/starlight';
import Icons from "unplugin-icons/vite";
import tailwind from '@astrojs/tailwind';
import IconsResolver from "unplugin-icons/resolver";
import Components from "unplugin-vue-components/vite";

import vue from '@astrojs/vue';

// https://astro.build/config
export default defineConfig({
	site: 'https://wpplumber.github.io/astroboard',
    base: 'astroboard',
	outDir: 'docs',
      image: {
    service: squooshImageService()
  },
      vite: {
    plugins: [ 
      Icons({
        autoInstall: true,
      }),
    ],
  },
    integrations: [starlight({
        title: 'Astroboard',
        logo: {
            light: '/public/images/logo.png',
            dark: '/public/images/logo.png',
            replacesTitle: false,
        },
        social: {
            github: 'https://github.com/wpplumber/astroboard',
        },
        sidebar: [
        	{
        		label: 'Start here',
        		items: [
        			{ label: 'Getting Started', slug: 'getting-started' },
        			{ label: 'Manual Setup', slug: 'manual-setup' },
        		],
        	},
        ],
        }), tailwind(), vue()],
});