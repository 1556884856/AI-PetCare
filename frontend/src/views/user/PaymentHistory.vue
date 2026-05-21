<template>
  <div class="page">
    <h1 class="page-title">💳 支付记录</h1>
    <div v-if="list.length === 0" class="empty">暂无支付记录</div>
    <div class="pay-list">
      <div v-for="p in list" :key="p.id" class="pay-item">
        <div class="pay-left">
          <div class="pay-service">{{ p.serviceName }}</div>
          <div class="pay-pet">🐾 {{ p.petName }}</div>
          <div class="pay-time">{{ formatTime(p.createdAt) }}</div>
        </div>
        <div class="pay-right">
          <div class="pay-amount">
            <span class="final">¥{{ p.finalAmount.toFixed(2) }}</span>
            <span v-if="p.discountAmount > 0" class="discount">省 ¥{{ p.discountAmount.toFixed(2) }}</span>
          </div>
          <el-tag :type="statusType(p.status)" size="small">{{ statusLabel(p.status) }}</el-tag>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getMyPayments } from '@/api/payments'

const list = ref<any[]>([])

onMounted(async () => {
  try {
    const res: any = await getMyPayments()
    list.value = res.data || []
  } catch {}
})

const statusLabel = (s: number) => {
  const map: Record<number, string> = { 0: '待支付', 1: '已支付', 2: '已退款' }
  return map[s] || '未知'
}

const statusType = (s: number) => {
  const map: Record<number, string> = { 0: 'warning', 1: 'success', 2: 'info' }
  return map[s] || 'info'
}

const formatTime = (t: string) => {
  const d = new Date(t)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}
</script>

<style scoped>
.page { max-width: 800px; margin: 0 auto; padding: 48px 24px; }
.page-title { font-size: 28px; margin-bottom: 24px; }
.empty { text-align: center; padding: 60px 0; color: #999; }
.pay-list { display: flex; flex-direction: column; gap: 12px; }
.pay-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}
.pay-service { font-size: 16px; font-weight: 600; }
.pay-pet { font-size: 14px; color: #666; margin-top: 4px; }
.pay-time { font-size: 12px; color: #bbb; margin-top: 4px; }
.pay-right { text-align: right; display: flex; flex-direction: column; align-items: flex-end; gap: 8px; }
.pay-amount { display: flex; align-items: baseline; gap: 6px; }
.final { font-size: 20px; font-weight: 700; color: var(--secondary); }
.discount { font-size: 13px; color: #999; text-decoration: line-through; }
</style>
