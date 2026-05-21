import request from '@/utils/request'

export const getAvailableCoupons = () =>
  request.get('/coupons/available')

export const getMyCoupons = (status?: string) =>
  request.get('/coupons/my', { params: { status } })

export const claimCoupon = (id: number) =>
  request.post(`/coupons/${id}/claim`)

export const getAllCoupons = () =>
  request.get('/admin/coupons')

export const createCoupon = (data: any) =>
  request.post('/admin/coupons', data)

export const updateCoupon = (id: number, data: any) =>
  request.put(`/admin/coupons/${id}`, data)

export const deleteCoupon = (id: number) =>
  request.delete(`/admin/coupons/${id}`)

export const distributeCoupon = (id: number, data: { userIds: number[] }) =>
  request.post(`/admin/coupons/${id}/distribute`, data)
