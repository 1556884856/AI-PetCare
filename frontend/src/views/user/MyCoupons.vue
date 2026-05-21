<template>
  <div class="page">
    <h1 class="page-title">🎫 我的优惠券</h1>
    <el-tabs v-model="activeTab" @tab-change="load">
      <el-tab-pane label="未使用" name="unused" />
      <el-tab-pane label="已使用" name="used" />
      <el-tab-pane label="已过期" name="expired" />
    </el-tabs>
    <div v-if="list.length === 0" class="empty">暂无优惠券</div>
    <div class="coupon-list">
      <div v-for="c in list" :key="c.id" class="coupon-card" :class="{ used: c.isUsed, expired: activeTab === 'expired' }">
        <div class="coupon-left">
          <div class="coupon-value">
            <template v-if="c.type === 0">¥{{ c.value }}</template>
            <template v-else>{{ (c.value * 10).toFixed(1) }}折</template>
          </div>
          <div class="coupon-name">{{ c.couponName }}</div>
        </div>
        <div class="coupon-right">
          <div class="coupon-condition" v-if="c.minOrderAmount > 0">满 ¥{{ c.minOrderAmount }} 可用</div>
          <div class="coupon-condition" v-else>无门槛</div>
          <div class="coupon-date">{{ formatDate(c.validFrom) }} ~ {{ formatDate(c.validTo) }}</div>
          <el-tag v-if="c.isUsed" type="info" size="small">已使用</el-tag>
          <el-tag v-else-if="activeTab === 'expired'" type="danger" size="small">已过期</el-tag>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getMyCoupons } from '@/api/coupons'

const activeTab = ref('unused')
const list = ref<any[]>([])

const load = async () => {
  try {
    const res: any = await getMyCoupons(activeTab.value)
    list.value = res.data || []
  } catch {}
}

load()

const formatDate = (t: string) => {
  return new Date(t).toLocaleDateString()
}
</script>

<style scoped>
.page { max-width: 800px; margin: 0 auto; padding: 48px 24px; }
.page-title { font-size: 28px; margin-bottom: 24px; }
.empty { text-align: center; padding: 60px 0; color: #999; }
.coupon-list { display: flex; flex-direction: column; gap: 12px; }
.coupon-card {
  display: flex;
  background: linear-gradient(135deg, #fff8e6, #fff);
  border: 2px solid #ffd666;
  border-radius: 12px;
  overflow: hidden;
}
.coupon-card.used, .coupon-card.expired {
  opacity: 0.6;
  filter: grayscale(0.5);
}
.coupon-left {
  width: 160px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: var(--secondary);
  color: #fff;
  padding: 24px 12px;
}
.coupon-card.used .coupon-left, .coupon-card.expired .coupon-left {
  background: #999;
}
.coupon-value { font-size: 32px; font-weight: 800; }
.coupon-name { font-size: 13px; margin-top: 4px; opacity: 0.9; }
.coupon-right {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 8px;
  padding: 24px 16px;
}
.coupon-condition { font-size: 14px; color: #666; font-weight: 500; }
.coupon-date { font-size: 12px; color: #999; }
</style>
