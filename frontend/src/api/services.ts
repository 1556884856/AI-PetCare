import request from '@/utils/request'

export const getServices = (params?: { category?: string; petType?: string }) =>
  request.get('/services', { params })

export const getServiceDetail = (id: number) =>
  request.get(`/services/${id}`)

export const createService = (data: any) =>
  request.post('/admin/services', data)

export const updateService = (id: number, data: any) =>
  request.put(`/admin/services/${id}`, data)

export const deleteService = (id: number) =>
  request.delete(`/admin/services/${id}`)
