// ===================================================================
// Vue Router 路由配置
// 前台路由（/）使用 FrontLayout，后台路由（/admin）使用 AdminLayout
// 路由守卫：检查认证状态和管理员权限
// ===================================================================
import { createRouter, createWebHistory } from 'vue-router'
const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('@/layouts/FrontLayout.vue'),
      children: [
        { path: '', name: 'Home', component: () => import('@/views/home/HomePage.vue') },
        { path: 'services', name: 'Services', component: () => import('@/views/services/ServiceList.vue') },
        { path: 'services/:id', name: 'ServiceDetail', component: () => import('@/views/services/ServiceDetail.vue') },
        { path: 'booking', name: 'Booking', component: () => import('@/views/booking/BookingPage.vue'), meta: { requiresAuth: true } },
        { path: 'payment/:id?', name: 'Payment', component: () => import('@/views/payment/PaymentPage.vue'), meta: { requiresAuth: true } },
        { path: 'login', name: 'Login', component: () => import('@/views/user/LoginPage.vue') },
        { path: 'me', name: 'Profile', component: () => import('@/views/user/ProfilePage.vue'), meta: { requiresAuth: true } },
        { path: 'me/pets', name: 'MyPets', component: () => import('@/views/user/PetList.vue'), meta: { requiresAuth: true } },
        { path: 'me/appointments', name: 'MyAppointments', component: () => import('@/views/user/AppointmentList.vue'), meta: { requiresAuth: true } },
        { path: 'me/notifications', name: 'MyNotifications', component: () => import('@/views/user/NotificationList.vue'), meta: { requiresAuth: true } },
        { path: 'me/coupons', name: 'MyCoupons', component: () => import('@/views/user/MyCoupons.vue'), meta: { requiresAuth: true } },
        { path: 'me/payments', name: 'MyPayments', component: () => import('@/views/user/PaymentHistory.vue'), meta: { requiresAuth: true } },
        { path: 'coupons', name: 'CouponCenter', component: () => import('@/views/user/CouponCenter.vue'), meta: { requiresAuth: true } }
      ]
    },
    {
      path: '/admin',
      component: () => import('@/layouts/AdminLayout.vue'),
      meta: { requiresAuth: true, requiresAdmin: true },
      children: [
        { path: '', name: 'Dashboard', component: () => import('@/views/admin/DashboardPage.vue') },
        { path: 'appointments', name: 'AdminAppointments', component: () => import('@/views/admin/AppointmentManage.vue') },
        { path: 'services', name: 'AdminServices', component: () => import('@/views/admin/ServiceManage.vue') },
        { path: 'customers', name: 'AdminCustomers', component: () => import('@/views/admin/CustomerManage.vue') },
        { path: 'pets', name: 'AdminPets', component: () => import('@/views/admin/PetManage.vue') },
        { path: 'coupons', name: 'AdminCoupons', component: () => import('@/views/admin/CouponManage.vue') }
      ]
    }
  ]
})
// 全局前置守卫：认证检查和管理员权限检查
router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next('/login')  // 未登录 → 跳转登录页
  } else if (to.meta.requiresAdmin) {
    const userStr = localStorage.getItem('user')
    if (!userStr) { next('/login'); return }
    const user = JSON.parse(userStr)
    if (user.role !== 2) { next('/'); return }  // 非管理员 → 跳转首页
    next()
  } else {
    next()
  }
})
export default router
