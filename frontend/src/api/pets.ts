import request from '@/utils/request'

export const getMyPets = () =>
  request.get('/pets')

export const createPet = (data: any) =>
  request.post('/pets', data)

export const updatePet = (id: number, data: any) =>
  request.put(`/pets/${id}`, data)

export const deletePet = (id: number) =>
  request.delete(`/pets/${id}`)

export const getAllPets = (params?: any) =>
  request.get('/admin/pets', { params })
