// ===================================================================
// Axios 请求封装
// - 自动附加 JWT Token 到请求头
// - 统一错误处理和 401 自动跳转登录
// ===================================================================
import axios from 'axios'
import { ElMessage } from 'element-plus'

const request = axios.create({
  baseURL: '/api/v1',  // API 基础路径
  timeout: 15000       // 请求超时时间 15 秒
})

// 请求拦截器：自动附带 Token
request.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = 'Bearer ' + token
  }
  return config
})

// 响应拦截器：统一解包 data 层，处理通用错误
request.interceptors.response.use(
  (res) => res.data,
  (error) => {
    const msg = error.response?.data?.message || '请求失败'
    ElMessage.error(msg)
    // 401 未认证 → 清除登录状态并跳转到登录页
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default request
