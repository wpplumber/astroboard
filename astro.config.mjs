// @ts-check
import { defineConfig, passthroughImageService } from "astro/config";
import starlight from "@astrojs/starlight";
import Icons from "unplugin-icons/vite";
import tailwindcss from "@tailwindcss/vite";

import vue from "@astrojs/vue";

export default defineConfig({
  image: {
    service: passthroughImageService(),
  },
  vite: {
    plugins: [
      tailwindcss(),

      Icons({
        autoInstall: true,
      }),
    ],
  },
  integrations: [
    starlight({
      customCss: ["/src/styles/global.css"],
      title: "Astroboard",
      logo: {
        light: "/public/images/logo.png",
        dark: "/public/images/logo.png",
        // replacesTitle: false,
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
    vue(),
  ],
});
