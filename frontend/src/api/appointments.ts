import request from '@/utils/request'

export const getMyAppointments = (params?: { status?: number }) =>
  request.get('/appointments', { params })

export const createAppointment = (data: any) =>
  request.post('/appointments', data)

export const cancelAppointment = (id: number) =>
  request.put(`/appointments/${id}/cancel`)

export const getAvailableSlots = (date: string) =>
  request.get('/appointments/available-slots', { params: { date } })

export const getAllAppointments = (params?: any) =>
  request.get('/admin/appointments', { params })

export const confirmAppointment = (id: number) =>
  request.put(`/admin/appointments/${id}/confirm`)

export const completeAppointment = (id: number) =>
  request.put(`/admin/appointments/${id}/complete`)

export const adminCancelAppointment = (id: number) =>
  request.put(`/admin/appointments/${id}/cancel`)
