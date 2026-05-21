import request from '@/utils/request'

export const getNotifications = (page = 1, pageSize = 20) =>
  request.get('/notifications', { params: { page, pageSize } })

export const getUnreadCount = () =>
  request.get('/notifications/unread-count')

export const markNotificationRead = (id: number) =>
  request.put(`/notifications/${id}/read`)

export const markAllNotificationsRead = () =>
  request.put('/notifications/read-all')

export const deleteNotification = (id: number) =>
  request.delete(`/notifications/${id}`)

export const sendBulkNotification = (data: any) =>
  request.post('/admin/notifications/send', data)
