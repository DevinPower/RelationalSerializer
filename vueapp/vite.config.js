import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'
import fs from 'fs'
import path from 'path'

const baseFolder =
  process.env.APPDATA && process.env.APPDATA !== ''
    ? `${process.env.APPDATA}/ASP.NET/https`
    : `${process.env.HOME}/.aspnet/https`

const certificateName = "vueapp"
const certFilePath = path.join(baseFolder, `${certificateName}.pem`)
const keyFilePath = path.join(baseFolder, `${certificateName}.key`)

// Check if certificate files exist
const certExists = fs.existsSync(certFilePath)
const keyExists = fs.existsSync(keyFilePath)
const httpsEnabled = certExists && keyExists

// Log certificate status
if (httpsEnabled) {
  console.log('✓ HTTPS certificates found, enabling HTTPS server')
} else {
  console.log('⚠ HTTPS certificates not found, running HTTP only')
  if (!certExists) console.log(`  Missing: ${certFilePath}`)
  if (!keyExists) console.log(`  Missing: ${keyFilePath}`)
}

export default defineConfig({
  plugins: [
    tailwindcss(),
    vue()
  ],
  server: {
    ...(httpsEnabled && {
      https: {
        key: fs.readFileSync(keyFilePath),
        cert: fs.readFileSync(certFilePath),
      }
    }),
    port: 5002,
    proxy: {
      '/api': {
        target: 'http://localhost:5033/',
        changeOrigin: true,
        ws: true,
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
})