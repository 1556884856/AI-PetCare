// ===================================================================
// 认证状态管理 (Pinia Store)
// 管理 JWT Token、用户信息、登录状态和角色判断
// ===================================================================
import { defineStore } from 'pinia'
import { ref } from 'vue'
export interface UserInfo {
  id: number
  phone: string
  nickname: string
  avatarUrl: string
  role: number  // 0=Customer, 1=Staff, 2=Admin
}
export const useAuthStore = defineStore('auth', () => {
  // 从 localStorage 恢复持久化的登录状态
  const token = ref<string>(localStorage.getItem('token') || '')
  const user = ref<UserInfo | null>(JSON.parse(localStorage.getItem('user') || 'null'))
  const isLoggedIn = () => !!token.value
  const isAdmin = () => user.value?.role === 2
  const setToken = (t: string) => { token.value = t; localStorage.setItem('token', t) }
  const setUser = (u: UserInfo) => { user.value = u; localStorage.setItem('user', JSON.stringify(u)) }
  const logout = () => { token.value = ''; user.value = null; localStorage.removeItem('token'); localStorage.removeItem('user') }
  return { token, user, isLoggedIn, isAdmin, setToken, setUser, logout }
})
