/** @type {import('tailwindcss').Config} */
export default {
  prefix: "tw-",
  content: ["./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}"],
  theme: {
    extend: {
      colors: {
        published: {
          light: "#2bc37c", // Custom light gray
          DEFAULT: "#2bc37c", // Custom default gray
          // dark: '#4B5563',        // Custom dark gray
        },
      },
    },
  },
  darkMode: "class",
  plugins: [
    require('@tailwindcss/typography'),
    require('@tailwindcss/forms'),
    require("daisyui")],
  daisyui: {
    // themes: ["light"],
    themes: [
      {
        light: {
          primary: "#3544b1",
          secondary: "#1b264f",
          accent: "#f5c1bc",
          neutral: "#3879ff",
          "base-100": "#f3f3f5",
          info: "#36419c",
          success: "#0b8152",
          warning: "#fbd142",
          error: "#d42054",
        },
      },
    ],
  },
};
