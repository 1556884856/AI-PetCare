<!--
  登录/注册页面
  手机号+验证码登录，首次登录自动注册。开发环境验证码固定为 1234。
-->
<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header"><span class="login-icon">🐾</span><h2>欢迎回来</h2><p>首次登录将自动注册账号</p></div>
      <el-form :model="form" label-position="top" size="large">
        <el-form-item label="手机号"><el-input v-model="form.phone" placeholder="请输入手机号" :prefix-icon="Phone" /></el-form-item>
        <el-form-item label="验证码">
          <div class="code-row">
            <el-input v-model="form.code" placeholder="请输入验证码" :prefix-icon="Lock" />
            <el-button :disabled="countdown > 0" @click="sendSmsCode" class="code-btn">{{ countdown > 0 ? `${countdown}秒后重发` : '获取验证码' }}</el-button>
          </div>
        </el-form-item>
        <el-button type="primary" class="submit-btn" :loading="loading" @click="handleLogin">登 录</el-button>
      </el-form>
      <p class="hint">开发环境验证码：1234</p>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { sendCode, login } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'
import { ElMessage } from 'element-plus'
import { Phone, Lock } from '@element-plus/icons-vue'
const router = useRouter()
const auth = useAuthStore()
const form = reactive({ phone: '', code: '' })
const loading = ref(false)
const countdown = ref(0)  // 验证码倒计时
const sendSmsCode = async () => {
  if (!/^1\d{10}$/.test(form.phone)) { ElMessage.warning('请输入正确的手机号'); return }
  try {
    await sendCode(form.phone)
    ElMessage.success('验证码已发送（开发环境：1234）')
    countdown.value = 60  // 60秒倒计时
    const timer = setInterval(() => { countdown.value--; if (countdown.value <= 0) clearInterval(timer) }, 1000)
  } catch {}
}
const handleLogin = async () => {
  if (!form.phone || !form.code) return ElMessage.warning('请填写手机号和验证码')
  loading.value = true
  try {
    const res: any = await login(form.phone, form.code)
    auth.setToken(res.data.token); auth.setUser(res.data.user)
    ElMessage.success('登录成功'); router.push('/')
  } catch {} finally { loading.value = false }
}
</script>
<style scoped>
.login-page { min-height: calc(100vh - 64px); display: flex; align-items: center; justify-content: center; background: linear-gradient(135deg, var(--primary-bg), #FEF3C7); }
.login-card { background: #fff; border-radius: var(--radius-lg); padding: 48px 40px; width: 400px; box-shadow: 0 8px 40px rgba(0,0,0,0.08); }
.login-header { text-align: center; margin-bottom: 32px; }
.login-icon { font-size: 48px; display: block; margin-bottom: 8px; }
.login-header h2 { font-size: 24px; margin-bottom: 4px; }
.login-header p { color: var(--muted); font-size: 14px; }
.code-row { display: flex; gap: 12px; }
.code-btn { flex-shrink: 0; }
.submit-btn { width: 100%; margin-top: 8px; }
.hint { text-align: center; color: var(--muted); font-size: 12px; margin-top: 16px; }
</style>
