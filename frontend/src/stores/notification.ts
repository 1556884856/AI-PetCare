import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getUnreadCount } from '@/api/notifications'

export const useNotificationStore = defineStore('notification', () => {
  const unreadCount = ref(0)
  let timer: ReturnType<typeof setInterval> | null = null

  const fetchUnread = async () => {
    try {
      const res: any = await getUnreadCount()
      unreadCount.value = res.data ?? 0
    } catch { /* ignore */ }
  }

  const startPolling = () => {
    fetchUnread()
    timer = setInterval(fetchUnread, 30000)
  }

  const stopPolling = () => {
    if (timer) { clearInterval(timer); timer = null }
  }

  const markRead = () => {
    if (unreadCount.value > 0) unreadCount.value--
  }

  const clearAll = () => { unreadCount.value = 0 }

  return { unreadCount, fetchUnread, startPolling, stopPolling, markRead, clearAll }
})
