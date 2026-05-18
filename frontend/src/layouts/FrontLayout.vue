<template>
  <div class="front-layout">
    <header class="header">
      <div class="header-inner">
        <router-link to="/" class="logo">
          <span class="logo-icon">🐾</span>
          <span class="logo-text">Pet Care</span>
        </router-link>
        <nav class="nav">
          <router-link to="/services">服务项目</router-link>
          <router-link to="/booking">在线预约</router-link>
        </nav>
        <div class="header-actions">
          <a v-if="auth.isAdmin()" href="/admin" target="_blank" class="admin-link">⚙ 后台管理</a>
          <template v-if="auth.isLoggedIn()">
            <el-dropdown>
              <span class="user-btn">
                <el-avatar :size="32" icon="UserFilled" />
                <span class="user-name">{{ auth.user?.nickname || auth.user?.phone }}</span>
              </span>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item @click="$router.push('/me')">个人中心</el-dropdown-item>
                  <el-dropdown-item @click="$router.push('/me/pets')">我的宠物</el-dropdown-item>
                  <el-dropdown-item @click="$router.push('/me/appointments')">我的预约</el-dropdown-item>
                  <el-dropdown-item v-if="auth.isAdmin()" @click="openAdmin" divided>后台管理</el-dropdown-item>
                  <el-dropdown-item @click="handleLogout" divided>退出登录</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </template>
          <router-link v-else to="/login" class="login-link">登录 / 注册</router-link>
        </div>
      </div>
    </header>
    <main class="main">
      <router-view />
    </main>
    <footer class="footer">
      <div class="footer-inner">
        <div class="footer-info">
          <h4>🐾 Pet Care</h4>
          <p>专业宠物洗护 · 用心呵护每一只毛孩子</p>
          <p>📍 地址：XX市XX区XX路123号</p>
          <p>📞 电话：400-XXX-XXXX</p>
          <p>🕐 营业时间：09:00 - 20:00（周一至周日）</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { getMe } from '@/api/auth'

const auth = useAuthStore()
const router = useRouter()

onMounted(async () => {
  if (auth.token) {
    try {
      const res: any = await getMe()
      auth.setUser(res.data)
    } catch {
      auth.logout()
    }
  }
})

const openAdmin = () => { window.open('/admin', '_blank') }
const handleLogout = () => {
  auth.logout()
  router.push('/')
}
</script>

<style scoped>
.header {
  background: var(--primary);
  color: #fff;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: 0 2px 12px rgba(0,0,0,0.1);
}
.header-inner {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
  height: 64px;
}
.logo {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #fff;
}
.logo-icon { font-size: 28px; }
.logo-text {
  font-family: 'Varela Round', sans-serif;
  font-size: 22px;
  font-weight: 700;
}
.nav {
  display: flex;
  gap: 32px;
}
.nav a {
  color: rgba(255,255,255,0.85);
  font-size: 15px;
  transition: color var(--transition);
}
.nav a:hover, .nav a.router-link-exact-active {
  color: #fff;
}
.header-actions { display: flex; align-items: center; }
.user-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  color: #fff;
}
.user-name { font-size: 14px; }
.login-link {
  color: #fff;
  font-size: 15px;
  padding: 6px 20px;
  border: 1.5px solid rgba(255,255,255,0.5);
  border-radius: var(--radius-full);
  transition: all var(--transition);
}
.login-link:hover { background: rgba(255,255,255,0.15); border-color: #fff; }
.admin-link {
  color: #fff;
  font-size: 14px;
  padding: 6px 16px;
  margin-right: 12px;
  background: rgba(255,255,255,0.15);
  border: 1.5px solid rgba(255,255,255,0.4);
  border-radius: var(--radius-full);
  transition: all var(--transition);
}
.admin-link:hover { background: rgba(255,255,255,0.25); border-color: #fff; }

.main { min-height: calc(100vh - 64px - 200px); }
.footer {
  background: #1C1917;
  color: rgba(255,255,255,0.7);
  padding: 48px 24px;
}
.footer-inner { max-width: 1200px; margin: 0 auto; }
.footer-info h4 {
  color: #fff;
  font-size: 18px;
  margin-bottom: 12px;
}
.footer-info p { margin-bottom: 6px; font-size: 14px; }
@media (max-width: 768px) {
  .nav { display: none; }
  .header-inner { padding: 0 16px; }
}
</style>
