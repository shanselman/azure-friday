/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    './Pages/**/*.cshtml',
    './wwwroot/js/**/*.js'
  ],
  theme: {
    extend: {
      colors: {
        azure: {
          50: '#e6f2ff',
          100: '#b3d9ff',
          200: '#80bfff',
          300: '#4da6ff',
          400: '#1a8cff',
          500: '#0078d4',
          600: '#005a9e',
          700: '#004578',
          800: '#002f52',
          900: '#001a2c',
        }
      }
    }
  },
  plugins: [],
}
