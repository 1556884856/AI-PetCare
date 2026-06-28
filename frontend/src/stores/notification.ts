// ===================================================================
// 通知状态管理 (Pinia Store)
// 轮询获取未读通知数 (每30秒)，供 NotificationBell 组件使用
// ===================================================================
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getUnreadCount } from '@/api/notifications'
export const useNotificationStore = defineStore('notification', () => {
  const unreadCount = ref(0)
  let timer: ReturnType<typeof setInterval> | null = null
  const fetchUnread = async () => {
    try { const res: any = await getUnreadCount(); unreadCount.value = res.data ?? 0 } catch {}
  }
  // 启动轮询：立即拉取一次，然后每30秒拉取
  const startPolling = () => { fetchUnread(); timer = setInterval(fetchUnread, 30000) }
  const stopPolling = () => { if (timer) { clearInterval(timer); timer = null } }
  // 本地减少未读数（已读单条时调用）
  const markRead = () => { if (unreadCount.value > 0) unreadCount.value-- }
  // 清空未读数（全部已读时调用）
  const clearAll = () => { unreadCount.value = 0 }
  return { unreadCount, fetchUnread, startPolling, stopPolling, markRead, clearAll }
})
