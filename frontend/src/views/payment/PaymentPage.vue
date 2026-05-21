<template>
  <div class="page">
    <h1 class="page-title">💳 确认支付</h1>

    <div v-if="payment" class="payment-card">
      <div class="order-summary">
        <h3>订单信息</h3>
        <div class="summary-row"><span>服务项目</span><strong>{{ payment.serviceName }}</strong></div>
        <div class="summary-row"><span>宠物</span><strong>{{ payment.petName }}</strong></div>
        <div class="summary-row"><span>原价</span><strong>¥{{ payment.amount.toFixed(2) }}</strong></div>
        <div class="summary-row discount" v-if="payment.discountAmount > 0">
          <span>优惠</span><strong>-¥{{ payment.discountAmount.toFixed(2) }}</strong>
        </div>
        <div class="summary-row total"><span>实付</span><strong class="price">¥{{ payment.finalAmount.toFixed(2) }}</strong></div>
      </div>

      <div class="pay-methods">
        <h3>支付方式</h3>
        <div class="method-list">
          <div
            class="method-item"
            :class="{ active: payMethod === 0 }"
            @click="payMethod = 0"
          >
            <span class="method-icon">💚</span>
            <span>微信支付</span>
          </div>
          <div
            class="method-item"
            :class="{ active: payMethod === 1 }"
            @click="payMethod = 1"
          >
            <span class="method-icon">💙</span>
            <span>支付宝</span>
          </div>
          <div
            class="method-item"
            :class="{ active: payMethod === 2 }"
            @click="payMethod = 2"
          >
            <span class="method-icon">🏪</span>
            <span>到店支付</span>
          </div>
        </div>
      </div>

      <div class="pay-actions">
        <el-button size="large" @click="$router.push('/me/appointments')">稍后支付</el-button>
        <el-button type="warning" size="large" :loading="paying" @click="doPay" :disabled="payStatus !== 0">
          {{ payStatus !== 0 ? '已支付' : '确认支付 ¥' + payment.finalAmount.toFixed(2) }}
        </el-button>
      </div>

      <div v-if="paying" class="paying-overlay">
        <div class="paying-spinner">
          <el-icon class="is-loading" :size="48"><Loading /></el-icon>
          <p>正在模拟支付中...</p>
        </div>
      </div>
    </div>

    <div v-else-if="!payment && !loading" class="empty">支付单不存在</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Loading } from '@element-plus/icons-vue'
import { createPayment, getPayment, mockPay } from '@/api/payments'
import { ElMessage } from 'element-plus'

const route = useRoute()
const router = useRouter()
const payment = ref<any>(null)
const paying = ref(false)
const loading = ref(true)
const payMethod = ref(0)
const payStatus = ref(0)

onMounted(async () => {
  const paymentId = route.params.id ? Number(route.params.id) : null
  const appointmentId = route.query.appointmentId ? Number(route.query.appointmentId) : null
  const couponId = route.query.couponId ? Number(route.query.couponId) : undefined

  try {
    if (paymentId) {
      const res: any = await getPayment(paymentId)
      payment.value = res.data
      payStatus.value = payment.value.status
    } else if (appointmentId) {
      const res: any = await createPayment({
        appointmentId,
        couponId,
        payMethod: payMethod.value
      })
      payment.value = res.data
      payStatus.value = payment.value.status
    }
  } catch {
    ElMessage.error('加载支付信息失败')
  } finally {
    loading.value = false
  }
})

const doPay = async () => {
  if (!payment.value) return
  paying.value = true
  try {
    await mockPay(payment.value.id)
    payStatus.value = 1
    ElMessage.success('支付成功！')
    setTimeout(() => {
      router.push('/me/appointments')
    }, 1000)
  } catch {} finally {
    paying.value = false
  }
}
</script>

<style scoped>
.page { max-width: 600px; margin: 0 auto; padding: 48px 24px; position: relative; }
.page-title { font-size: 28px; text-align: center; margin-bottom: 32px; }
.empty { text-align: center; padding: 60px 0; color: #999; }
.payment-card { background: #fff; border-radius: 16px; padding: 32px; box-shadow: var(--shadow-card); }
.order-summary { margin-bottom: 32px; }
.order-summary h3, .pay-methods h3 { font-size: 16px; margin-bottom: 16px; color: #333; }
.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #f0f0f0;
  font-size: 15px;
}
.summary-row span { color: #666; }
.summary-row.discount strong { color: var(--secondary); }
.summary-row.total { border-bottom: none; font-size: 18px; }
.price { color: var(--secondary); font-size: 24px; }
.method-list { display: flex; gap: 12px; }
.method-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 16px;
  border: 2px solid var(--border);
  border-radius: 12px;
  cursor: pointer;
  transition: all var(--transition);
  font-size: 14px;
}
.method-item:hover { border-color: var(--primary); }
.method-item.active { border-color: var(--primary); background: var(--primary-bg); }
.method-icon { font-size: 28px; }
.pay-actions { display: flex; justify-content: center; gap: 16px; margin-top: 32px; }
.paying-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.paying-spinner {
  background: #fff;
  border-radius: 16px;
  padding: 48px;
  text-align: center;
}
.paying-spinner p { margin-top: 16px; color: #666; font-size: 15px; }
</style>
