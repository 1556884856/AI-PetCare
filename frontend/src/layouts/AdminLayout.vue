<template>
  <div class="admin-layout">
    <aside class="sidebar">
      <div class="sidebar-header">
        <router-link to="/admin" class="sidebar-logo">🐾 后台管理</router-link>
      </div>
      <el-menu
        :default-active="$route.path"
        router
        background-color="#0F766E"
        text-color="rgba(255,255,255,0.8)"
        active-text-color="#fff"
        class="sidebar-menu"
      >
        <el-menu-item index="/admin">
          <el-icon><DataAnalysis /></el-icon>
          <span>仪表盘</span>
        </el-menu-item>
        <el-menu-item index="/admin/appointments">
          <el-icon><Calendar /></el-icon>
          <span>预约管理</span>
        </el-menu-item>
        <el-menu-item index="/admin/services">
          <el-icon><MagicStick /></el-icon>
          <span>服务管理</span>
        </el-menu-item>
        <el-menu-item index="/admin/coupons">
          <el-icon><Ticket /></el-icon>
          <span>优惠券管理</span>
        </el-menu-item>
        <el-menu-item index="/admin/customers">
          <el-icon><User /></el-icon>
          <span>客户管理</span>
        </el-menu-item>
        <el-menu-item index="/admin/pets">
          <el-icon><DishDot /></el-icon>
          <span>宠物管理</span>
        </el-menu-item>
        <el-menu-item @click="goHome">
          <el-icon><HomeFilled /></el-icon>
          <span>返回前台</span>
        </el-menu-item>
      </el-menu>
    </aside>
    <div class="admin-main">
      <router-view />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
const router = useRouter()
const goHome = () => router.push('/')
</script>

<style scoped>
.admin-layout { display: flex; min-height: 100vh; }
.sidebar {
  width: 220px;
  background: var(--primary);
  flex-shrink: 0;
}
.sidebar-header {
  padding: 20px;
  border-bottom: 1px solid rgba(255,255,255,0.1);
}
.sidebar-logo {
  color: #fff;
  font-family: 'Varela Round', sans-serif;
  font-size: 18px;
  font-weight: 700;
}
.sidebar-menu { border-right: none; }
.admin-main {
  flex: 1;
  background: #f5f5f5;
  padding: 24px;
  overflow-y: auto;
}
@media (max-width: 768px) {
  .sidebar { width: 60px; }
  .sidebar-header { padding: 12px 8px; }
  .sidebar-logo { font-size: 14px; }
  .sidebar-menu span { display: none; }
}
</style>
