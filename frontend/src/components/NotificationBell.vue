<!--
  通知铃铛组件
  显示未读通知徽标，点击展开下拉通知面板（最近5条）
  挂载时启动30秒轮询，卸载时停止
-->
<template>
  <div class="notification-bell" @click="toggle">
    <el-badge :value="store.unreadCount" :hidden="store.unreadCount === 0" :max="99">
      <span class="bell-icon">🔔</span>
    </el-badge>
    <div v-if="show" class="bell-dropdown" @click.stop>
      <div class="bell-header">
        <span>消息通知</span>
        <el-button v-if="store.unreadCount > 0" type="primary" link size="small" @click="readAll">全部已读</el-button>
      </div>
      <div class="bell-list">
        <div v-if="notifications.length === 0" class="bell-empty">暂无通知</div>
        <div v-for="n in notifications.slice(0, 5)" :key="n.id" class="bell-item" :class="{ unread: !n.isRead }" @click="handleClick(n)">
          <div class="bell-item-title">{{ n.title }}</div>
          <div class="bell-item-content">{{ n.content }}</div>
          <div class="bell-item-time">{{ formatTime(n.createdAt) }}</div>
        </div>
      </div>
      <div class="bell-footer"><router-link to="/me/notifications" @click="show = false">查看全部 →</router-link></div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '@/stores/notification'
import { getNotifications, markNotificationRead, markAllNotificationsRead } from '@/api/notifications'
const store = useNotificationStore()
const router = useRouter()
const show = ref(false)
const notifications = ref<any[]>([])
onMounted(async () => {
  store.startPolling()
  document.addEventListener('click', handleOutside)
  await loadNotifications()
})
onUnmounted(() => { store.stopPolling(); document.removeEventListener('click', handleOutside) })
const handleOutside = () => { show.value = false }
const toggle = () => { show.value = !show.value; if (show.value) loadNotifications() }
const loadNotifications = async () => { try { const res: any = await getNotifications(1, 5); notifications.value = res.data || [] } catch {} }
// 点击通知：标记已读，如果是预约相关通知则跳转到预约列表
const handleClick = async (n: any) => {
  if (!n.isRead) { await markNotificationRead(n.id); store.markRead(); n.isRead = true }
  if (n.relatedId && n.type === 0) { router.push('/me/appointments') }
  show.value = false
}
const readAll = async () => { await markAllNotificationsRead(); store.clearAll(); notifications.value.forEach(n => n.isRead = true) }
const formatTime = (t: string) => { const d = new Date(t); return d.toLocaleDateString() + ' ' + d.toLocaleTimeString() }
</script>
<style scoped>
.notification-bell { position: relative; cursor: pointer; margin-right: 16px; }
.bell-icon { font-size: 22px; }
.bell-dropdown { position: absolute; top: 44px; right: -12px; width: 360px; background: #fff; border-radius: 12px; box-shadow: 0 8px 32px rgba(0,0,0,0.15); z-index: 200; overflow: hidden; }
.bell-header { display: flex; justify-content: space-between; align-items: center; padding: 16px 20px; border-bottom: 1px solid #f0f0f0; font-weight: 600; color: #333; }
.bell-list { max-height: 360px; overflow-y: auto; }
.bell-empty { padding: 40px; text-align: center; color: #999; font-size: 14px; }
.bell-item { padding: 14px 20px; border-bottom: 1px solid #f5f5f5; cursor: pointer; transition: background var(--transition); }
.bell-item:hover { background: #f9f9f9; }
.bell-item.unread { background: #fef7e8; }
.bell-item-title { font-size: 14px; font-weight: 600; color: #333; margin-bottom: 4px; }
.bell-item-content { font-size: 13px; color: #666; margin-bottom: 4px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis; }
.bell-item-time { font-size: 12px; color: #bbb; }
.bell-footer { padding: 12px 20px; text-align: center; border-top: 1px solid #f0f0f0; }
.bell-footer a { font-size: 14px; color: var(--primary); }
</style>
