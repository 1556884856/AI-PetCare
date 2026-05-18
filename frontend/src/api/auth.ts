import request from '@/utils/request'

export const sendCode = (phone: string) =>
  request.post('/auth/send-code', { phone })

export const login = (phone: string, code: string) =>
  request.post('/auth/login', { phone, code })

export const getMe = () =>
  request.get('/auth/me')

export const updateProfile = (data: { nickname?: string }) =>
  request.put('/auth/me', data)
