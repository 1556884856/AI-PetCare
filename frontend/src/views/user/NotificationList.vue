<template>
  <div class="page">
    <h1 class="page-title">🔔 消息通知</h1>
    <div class="toolbar">
      <el-button type="primary" link @click="markAll">全部已读</el-button>
    </div>
    <div v-if="list.length === 0" class="empty">暂无通知</div>
    <div v-for="n in list" :key="n.id" class="notif-item" :class="{ unread: !n.isRead }">
      <div class="notif-left" @click="handleClick(n)">
        <span class="notif-dot" v-if="!n.isRead"></span>
        <div>
          <div class="notif-title">
            <span class="notif-tag">{{ typeLabel(n.type) }}</span>
            {{ n.title }}
          </div>
          <div class="notif-content">{{ n.content }}</div>
          <div class="notif-time">{{ formatTime(n.createdAt) }}</div>
        </div>
      </div>
      <el-button type="danger" link size="small" @click="remove(n.id)">删除</el-button>
    </div>
    <div class="pager">
      <el-pagination
        v-model:current-page="page"
        :page-size="20"
        layout="prev, pager, next"
        :total="total"
        @current-change="load"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '@/stores/notification'
import { getNotifications, markNotificationRead, markAllNotificationsRead, deleteNotification } from '@/api/notifications'

const router = useRouter()
const store = useNotificationStore()
const list = ref<any[]>([])
const page = ref(1)
const total = ref(0)

const load = async () => {
  try {
    const res: any = await getNotifications(page.value, 20)
    list.value = res.data || []
    total.value = 100
  } catch {}
}

load()

const handleClick = async (n: any) => {
  if (!n.isRead) {
    await markNotificationRead(n.id)
    store.markRead()
    n.isRead = true
  }
  if (n.relatedId && n.type === 0) {
    router.push('/me/appointments')
  }
}

const markAll = async () => {
  await markAllNotificationsRead()
  store.clearAll()
  list.value.forEach(n => n.isRead = true)
}

const remove = async (id: number) => {
  await deleteNotification(id)
  list.value = list.value.filter(n => n.id !== id)
}

const typeLabel = (t: number) => {
  const map: Record<number, string> = { 0: '预约', 1: '状态', 2: '促销', 3: '系统' }
  return map[t] || '其他'
}

const formatTime = (t: string) => {
  const d = new Date(t)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}
</script>

<style scoped>
.page { max-width: 800px; margin: 0 auto; padding: 48px 24px; }
.page-title { font-size: 28px; margin-bottom: 24px; }
.toolbar { margin-bottom: 16px; text-align: right; }
.empty { text-align: center; padding: 60px 0; color: #999; }
.notif-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 16px 20px;
  background: #fff;
  border-radius: 8px;
  margin-bottom: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}
.notif-item.unread { background: #fef7e8; }
.notif-left { display: flex; align-items: flex-start; gap: 12px; cursor: pointer; flex: 1; }
.notif-dot { display: inline-block; width: 8px; height: 8px; border-radius: 50%; background: var(--secondary); margin-top: 8px; flex-shrink: 0; }
.notif-title { font-size: 15px; font-weight: 600; margin-bottom: 4px; }
.notif-tag {
  display: inline-block;
  font-size: 11px;
  padding: 1px 6px;
  border-radius: 4px;
  background: var(--primary-bg);
  color: var(--primary);
  margin-right: 6px;
}
.notif-content { font-size: 14px; color: #666; margin-bottom: 4px; }
.notif-time { font-size: 12px; color: #bbb; }
.pager { display: flex; justify-content: center; margin-top: 24px; }
</style>
