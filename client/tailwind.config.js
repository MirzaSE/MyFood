/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
   plugins: [
    plugin(function ({ addBase }) {
      addBase({
        'label': {
          padding: '10px 8px', // or use theme values
        },
      })
    }),
  ],
}
