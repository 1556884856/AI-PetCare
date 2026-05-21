import request from '@/utils/request'

export const createPayment = (data: { appointmentId: number; couponId?: number; payMethod: number }) =>
  request.post('/payments', data)

export const mockPay = (id: number) =>
  request.put(`/payments/${id}/pay`)

export const getPayment = (id: number) =>
  request.get(`/payments/${id}`)

export const getMyPayments = () =>
  request.get('/payments/my')

export const getAllPayments = () =>
  request.get('/admin/payments')

export const refundPayment = (id: number) =>
  request.post(`/admin/payments/${id}/refund`)
