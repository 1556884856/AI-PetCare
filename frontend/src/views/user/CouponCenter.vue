<!--
  领券中心页面
  展示所有可领取的优惠券，点击"立即领取"调用 API
-->
<template>
  <div class="page"><h1 class="page-title">🎫 领券中心</h1>
    <div v-if="list.length === 0" class="empty">暂无可领取的优惠券</div>
    <div class="coupon-grid">
      <div v-for="c in list" :key="c.id" class="coupon-card">
        <div class="coupon-left">
          <div class="coupon-value"><template v-if="c.type === 0">¥{{ c.value }}</template><template v-else>{{ (c.value * 10).toFixed(1) }}折</template></div>
          <div class="coupon-name">{{ c.name }}</div>
        </div>
        <div class="coupon-right">
          <div class="coupon-condition" v-if="c.minOrderAmount > 0">满 ¥{{ c.minOrderAmount }} 可用</div>
          <div class="coupon-condition" v-else>无门槛</div>
          <div class="coupon-date">{{ formatDate(c.validFrom) }} ~ {{ formatDate(c.validTo) }}</div>
          <el-button type="warning" size="small" @click="claim(c.id)">立即领取</el-button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue'
import { getAvailableCoupons, claimCoupon } from '@/api/coupons'
import { ElMessage } from 'element-plus'
const list = ref<any[]>([])
const load = async () => { try { const res: any = await getAvailableCoupons(); list.value = res.data || [] } catch {} }
load()
const claim = async (id: number) => { try { await claimCoupon(id); ElMessage.success('领取成功！'); load() } catch {} }
const formatDate = (t: string) => new Date(t).toLocaleDateString()
</script>
<style scoped>
.page { max-width: 900px; margin: 0 auto; padding: 48px 24px; }
.page-title { font-size: 28px; margin-bottom: 24px; }
.empty { text-align: center; padding: 60px 0; color: #999; }
.coupon-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 16px; }
.coupon-card { display: flex; background: linear-gradient(135deg, #fff8e6, #fff); border: 2px solid #ffd666; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 16px rgba(255, 153, 0, 0.1); }
.coupon-left { width: 140px; display: flex; flex-direction: column; align-items: center; justify-content: center; background: var(--secondary); color: #fff; padding: 24px 12px; }
.coupon-value { font-size: 32px; font-weight: 800; }
.coupon-name { font-size: 13px; margin-top: 4px; opacity: 0.9; }
.coupon-right { flex: 1; display: flex; flex-direction: column; justify-content: center; align-items: center; gap: 8px; padding: 24px 16px; }
.coupon-condition { font-size: 14px; color: #666; font-weight: 500; }
.coupon-date { font-size: 12px; color: #999; }
@media (max-width: 768px) { .coupon-grid { grid-template-columns: 1fr; } }
</style>
