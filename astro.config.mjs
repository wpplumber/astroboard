// @ts-check
import { defineConfig } from "astro/config";
import starlight from "@astrojs/starlight";
import Icons from "unplugin-icons/vite";
import tailwind from "@astrojs/tailwind";

import vue from "@astrojs/vue";

export default defineConfig({
  vite: {
    plugins: [
      Icons({
        autoInstall: true,
      }),
    ],
  },
  integrations: [
    starlight({
      title: "Astroboard",
      logo: {
        light: "/public/images/logo.png",
        dark: "/public/images/logo.png",
        replacesTitle: false,
      },
      social: [
        {
          icon: "github",
          href: "https://github.com/wpplumber/astroboard",
          label: "GitHub",
        },
      ],
      sidebar: [
        {
          label: "Start here",
          items: [
            { label: "Getting Started", slug: "getting-started" },
            { label: "Manual Setup", slug: "manual-setup" },
          ],
        },
      ],
    }),
    tailwind(),
    vue(),
  ],
});
