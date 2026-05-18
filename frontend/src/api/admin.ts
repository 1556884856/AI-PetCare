import request from '@/utils/request'

export const getDashboard = () =>
  request.get('/admin/dashboard')

export const getCustomers = (params?: any) =>
  request.get('/admin/customers', { params })
