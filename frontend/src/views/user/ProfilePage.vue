<template>
  <div class="me-page">
    <div class="me-layout">
      <aside class="me-nav">
        <div class="user-card">
          <el-avatar :size="56" icon="UserFilled" />
          <div>
            <h3>{{ auth.user?.nickname || auth.user?.phone }}</h3>
            <p>{{ auth.user?.phone }}</p>
          </div>
        </div>
        <div class="nav-links">
          <router-link to="/me" class="nav-link" :class="{ active: $route.path === '/me' }">个人信息</router-link>
          <router-link to="/me/pets" class="nav-link" :class="{ active: $route.path === '/me/pets' }">我的宠物</router-link>
          <router-link to="/me/appointments" class="nav-link" :class="{ active: $route.path === '/me/appointments' }">我的预约</router-link>
        </div>
      </aside>
      <main class="me-content">
        <h2>个人信息</h2>
        <el-form :model="form" label-position="top" style="max-width: 400px;">
          <el-form-item label="昵称">
            <el-input v-model="form.nickname" placeholder="设置昵称" />
          </el-form-item>
          <el-form-item label="手机号">
            <el-input :model-value="auth.user?.phone" disabled />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="save">保存修改</el-button>
          </el-form-item>
        </el-form>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { updateProfile } from '@/api/auth'
import { ElMessage } from 'element-plus'

const auth = useAuthStore()
const form = reactive({ nickname: auth.user?.nickname || '' })

const save = async () => {
  try {
    const res: any = await updateProfile({ nickname: form.nickname })
    auth.setUser(res.data)
    ElMessage.success('保存成功')
  } catch {}
}
</script>

<style scoped>
.me-page { max-width: 1200px; margin: 0 auto; padding: 48px 24px; }
.me-layout { display: flex; gap: 32px; }
.me-nav { width: 220px; flex-shrink: 0; }
.user-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 20px;
  background: #fff;
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-card);
  margin-bottom: 12px;
}
.user-card h3 { font-size: 15px; }
.user-card p { color: var(--muted); font-size: 13px; }
.nav-links { background: #fff; border-radius: var(--radius-md); box-shadow: var(--shadow-card); overflow: hidden; }
.nav-link {
  display: block;
  padding: 14px 20px;
  font-size: 14px;
  border-left: 3px solid transparent;
  transition: all var(--transition);
}
.nav-link:hover { background: var(--primary-bg); }
.nav-link.active { border-left-color: var(--primary); color: var(--primary); background: var(--primary-bg); font-weight: 600; }
.me-content { flex: 1; background: #fff; border-radius: var(--radius-md); padding: 32px; box-shadow: var(--shadow-card); }
.me-content h2 { font-size: 22px; margin-bottom: 24px; }
@media (max-width: 768px) { .me-layout { flex-direction: column; } .me-nav { width: 100%; } }
</style>
