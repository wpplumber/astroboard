import { defineConfig } from "astro/config";
import tailwind from "@astrojs/tailwind";
import Icons from "unplugin-icons/vite";
import vue from "@astrojs/vue";
import IconsResolver from "unplugin-icons/resolver";
import Components from "unplugin-vue-components/vite";
import removeConsole from "vite-plugin-remove-console";

// https://astro.build/config
export default defineConfig({
  integrations: [tailwind(), vue()],
  base:
    process.env.NODE_ENV === "development"
      ? "."
      : "App_Plugins/Astroboard/astroboard/dist",
  vite: {
    plugins: [
      removeConsole(),
      Components({
        resolvers: [IconsResolver()],
      }),
      Icons({
        autoInstall: true,
        // compiler: 'astro',
      }),
    ],
  },
});
