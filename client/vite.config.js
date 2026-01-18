import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3000,
    host: '0.0.0.0', // Listen on all network interfaces (IPv4 and IPv6)
    open: false, // Don't auto-open browser
    proxy: {
      '/api': {
        target: 'http://localhost:5176',
        changeOrigin: true,
      }
    }
  }
})
