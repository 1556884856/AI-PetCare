import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface UserInfo {
  id: number
  phone: string
  nickname: string
  avatarUrl: string
  role: number
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string>(localStorage.getItem('token') || '')
  const user = ref<UserInfo | null>(JSON.parse(localStorage.getItem('user') || 'null'))

  const isLoggedIn = () => !!token.value
  const isAdmin = () => user.value?.role === 2

  const setToken = (t: string) => {
    token.value = t
    localStorage.setItem('token', t)
  }

  const setUser = (u: UserInfo) => {
    user.value = u
    localStorage.setItem('user', JSON.stringify(u))
  }

  const logout = () => {
    token.value = ''
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  return { token, user, isLoggedIn, isAdmin, setToken, setUser, logout }
})
