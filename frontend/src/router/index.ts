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

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else if (to.meta.requiresAdmin) {
    const userStr = localStorage.getItem('user')
    if (!userStr) { next('/login'); return }
    const user = JSON.parse(userStr)
    if (user.role !== 2) { next('/'); return }
    next()
  } else {
    next()
  }
})

export default router
